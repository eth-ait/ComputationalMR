using System;
using System.Collections.Generic;
using ConstraintUI.Model;
using ConstraintUI.Util;
using Gurobi;

namespace ConstraintUI.Optimization
{
    public class Optimizer
    {
        private readonly OptimizationModel _model;

        public Optimizer(OptimizationModel model)
        {
            _model = model;
        }

        public void StartOptimization()
        {
            try
            {
                RunOptimization();
            }
            catch (GRBException e)
            {
                Console.WriteLine(@"Error code: " + e.ErrorCode + @". " + e.Message);
            }
        }

        private void RunOptimization()
        {
            GRBEnv env = new GRBEnv();
            GRBModel model = new GRBModel(env);

            List<List<GRBVar>> varsFromElements = new List<List<GRBVar>>();

            //Variables: LoD for each element with their respective utility value
            for (var i = 0; i < _model.Elements.Count; i++)
            {
                var element = _model.Elements[i];
                var allLoads = CalcUtil.GetAllCognitiveLoadsForElement(element);

                List<GRBVar> varsCurrentElement = new List<GRBVar>();

                for (var j = 0; j < allLoads.Count; j++)
                {
                    //Fix element and current LoD if flag is set, i.e. lower and upper bound are 1.0
                    var lowerBound = element.FixateElement && (j + 1) == element.CurrentLevelOfDetail ? 1.0 : 0.0;

                    //Omit element if omit-flag is set, ie lower and upper bound are 0.0
                    var upperBound = !element.OmitElement ? 1.0 : 0.0;

                    //Disable changing the LoD by more than 1, ie set upper bound to 0.0 (except if element is fixated)
                    var absLoDDifference = Math.Abs(element.CurrentLevelOfDetail - 1 - j);
                    if (lowerBound < 1.0 && absLoDDifference > 1)
                        upperBound = 0.0;

                    GRBVar eLod = model.AddVar(lowerBound, upperBound, CalcUtil.GetObjectiveForElement(element, j + 1, _model), GRB.BINARY, "e_" + i + "_lod_" + j);
                    varsCurrentElement.Add(eLod);
                }

                varsFromElements.Add(varsCurrentElement);
            }

            //Constraint: overall cognitive load must be below user's cognitive capacity
            GRBLinExpr lhs = 0.0;
            for (var i = 0; i < _model.Elements.Count; i++)
            {
                var element = _model.Elements[i];
                var allLoads = CalcUtil.GetAllCognitiveLoadsForElement(element);

                for (var j = 0; j < allLoads.Count; j++)
                {
                    var load = allLoads[j] + CalcUtil.GetTimeDependentCognitiveLoadOnsetPenalty(element, _model);
                    lhs.AddTerm(load, varsFromElements[i][j]);
                }
            }
            model.AddConstr(lhs, GRB.LESS_EQUAL, _model.UserModel.CognitiveCapacity, "c_cog");

            //Constraint: constraints to number of available slots
            lhs = 0.0;
            for (var i = 0; i < _model.Elements.Count; i++)
            {
                for (var j = 0; j < _model.Elements[i].MaxLevelOfDetail; j++)
                {
                    lhs.AddTerm(1.0, varsFromElements[i][j]);
                }
            }

            model.AddConstr(lhs, GRB.GREATER_EQUAL, _model.NumMinPlacementSlots, "c_slots_min");
            model.AddConstr(lhs, GRB.LESS_EQUAL, _model.NumMaxPlacementSlots, "c_slots_max");

            //Constraint: only 1 LoD per Element
            for (var i = 0; i < _model.Elements.Count; i++)
            {
                lhs = 0.0;

                for (var j = 0; j < _model.Elements[i].MaxLevelOfDetail; j++)
                {
                    lhs.AddTerm(1.0, varsFromElements[i][j]);
                }

                model.AddConstr(lhs, GRB.LESS_EQUAL, 1.0, "c" + i + "_lod");
            }

            model.ModelSense = GRB.MAXIMIZE;

            model.Optimize();

            var status = model.Status;
            _model.IsFeasible = !(status == GRB.Status.INF_OR_UNBD || status == GRB.Status.INFEASIBLE || status == GRB.Status.UNBOUNDED);

            if (_model.IsFeasible)
            {

                UpdateModels(model, varsFromElements);
                StoreAllSolutions(model);
            }
            else
            {
                Console.WriteLine(@"The model cannot be solved because it is infeasible or unbounded");
            }

            model.Dispose();
            env.Dispose();
        }

        private void StoreAllSolutions(GRBModel model)
        {
            _model.Solutions.Clear();

            for (var i = 0; i < model.SolCount; i++)
            {
                model.Parameters.SolutionNumber = i;

                var vars = model.GetVars();

                var solution = new SolutionModel();
                var currentSolutionObjective = 0.0;

                for (var j = 0; j < model.NumVars; ++j)
                {
                    var sv = vars[j];

                    var name = sv.VarName;
                    var splitName = name.Split('_');
                    var id = int.Parse(splitName[1]);
                    var lod = int.Parse(splitName[3]);
                    var visibility = sv.Xn;

                    solution.AddEntry(id, lod, visibility);

                    var objective = sv.Obj * sv.Xn;
                    currentSolutionObjective += objective;
                }

                solution.Objective = currentSolutionObjective;
                _model.Solutions.Add(solution);
            }
        }

        private void UpdateModels(GRBModel model, List<List<GRBVar>> varsFromElements)
        {
            if (Constants.ENABLE_DEBUG_OUTPUT)
                OutputDebugVars(model);

            for (var i = 0; i < varsFromElements.Count; i++)
            {
                var currentElement = _model.Elements[i];
                var currentElementVars = varsFromElements[i];
                var lod = -1;

                for (var j = 0; j < currentElementVars.Count; j++)
                {
                    if (currentElementVars[j].X > 1e-6)
                    {
                        lod = j + 1;
                    }
                }

                if (lod > -1)
                {
                    currentElement.Visibility = 1.0;
                    currentElement.CurrentLevelOfDetail = lod;
                }
                else
                {
                    currentElement.Visibility = 0.0;
                }
            }
        }

        private static void OutputDebugVars(GRBModel model)
        {
            for (var s = 0; s < model.SolCount; s++)
            {
                model.Parameters.SolutionNumber = s;

                GRBVar[] vars = model.GetVars();
                for (var i = 0; i < model.NumVars; ++i)
                {
                    GRBVar sv = vars[i];
                    if (sv.Xn > 1e-6)
                    {
                        Console.WriteLine(@"s_" + s + @"  " + sv.VarName + @" = " + sv.Xn);
                    }
                }
            }
        }
    }
}