# Context-Aware Online Adaptation of Mixed Reality Interfaces
![Image of ConstraintUI testbox](images/compMR-teaser.jpg)

**Authors**: [David Lindlbauer](https://ait.ethz.ch/people/lindlbauer/), [Anna Maria Feit](https://ait.ethz.ch/people/feitan/), [Otmar Hilliges](https://ait.ethz.ch/people/hilliges/)  
**Publication**: [ACM UIST](https://uist.acm.org/uist2019/), New Orleans, LA, USA, October 2019  
**Project page**: [https://ait.ethz.ch/projects/2019/computationalMR/](https://ait.ethz.ch/projects/2019/computationalMR/)

## Research project description
We present an optimization-based approach for Mixed Reality (MR) systems to automatically control when and where applications are shown, and how much information they display. Currently, content creators design applications, and users then manually adjust which applications are visible and how much information they show. This choice has to be adjusted every time users switch context, i.e., whenever they switch their task or environment. Since context switches happen many times a day, we believe that MR interfaces require automation to alleviate this problem. We propose a real-time approach to automate this process based on users' current cognitive load and knowledge about their task and environment. Our system adapts which applications are displayed, how much information they show, and where they are placed. We formulate this problem as a mix of rule-based decision making and combinatorial optimization which can be solved efficiently in real-time. We present a set of proof-of-concept applications showing that our approach is applicable in a wide range of scenarios.

## Code
This repository contains the code published alongside with our UIST 2019 [paper](https://ait.ethz.ch/projects/2019/computationalMR/downloads/computationalMR_preprint.pdf). It is organised as follows: [`ConstraintUI`](ConstraintUI) contains an example of a 2D UI that is optimized using integer linear programming based on adjustable parameters. [`CompMR`](CompMR) contains Unity and Python scripts to run the optimization in real time.

### ConstraintUI Testbox
ConstraintUI is a small 2D test box to get started with constraint optimization, specifically integer linear programming. It is written in C# with Windows Presentation Foundation as UI framework. **It will therefore only run on Windows (tested with Win 10).** For more examples, visit the [Gurobi examples website](https://www.gurobi.com/documentation/8.0/examples/index.html).

##### Optimization (Gurobi)
For the optimization, we use the [Gurobi Optimizer 8.1](https://www.gurobi.com/). It is free for academic use. Please download and install it. The copy the files **Gurobi81.NET.dll** and **Gurobi81.NET.xml** from the folder *$GUROBI_INSTALL_DIR\win64\bin* into the folder *ComputationalMR\ConstraintUI\bin\Debug* (or *\Release*).

We use **Visual Studio 2017**, please open *ConstraintUI.sln* and hit build+run. **Note that it only works in 64bit configuration.**

##### Extended WPF Toolkit
For the interface, the *Extended Wpf Toolkit 3.4.0* is a also required. To install it, please open the VS solution, right-click on *References* and select *Manage NuGet packages*. On the top you will see a prompt that some packages are missing. Click *Restore* to install the missing packages.

This is how the software looks like:

![Image of ConstraintUI testbox](images/constraintUI-testbox.PNG)


The main code for the optimization is in *Optimization\Optimizer.cs*. On the top left side are the constraints (cognitive capacity and maximum number of slots to be filled), in the individual elements their input variables (importance and cognitive load when displayed, *MaxCogLoad*). You can run the optimization once by clicking the *Optimize* button, or continuously every time the cognitive capacity constraint changes it value (slider on the top).

*More detailed information will follow.*


## Computational MR
The main code of the paper. Tested with Unity 2018.3.6f1.
The project consists of two main scenes, *MainScene-AR* and *MainScene-VR*. Both use similar mechanisms, the former uses the ZED RGB-D camera to deliver a MR pass-through experience, whereas the later is purely in VR.  

Each scene consists of a set of applications, which are optimized based on the current cognitive load. They can be manually placed by the user using the controller. The main object that handles the application is the *AppController*.

*More detailed information will follow.*

Here is an example with as little as possible constraints of how to use the Unity project.  
1) Change the task  
2) Start Unity  
3) Optimize once  
4) Change "cognitive capacity" manually (usually automatically)
5) Optimize once  
6) Change to auto-optimize everytime the cognitive capacity undergoes a meaningful changes
7) Change cognitive capacity

![Image of ConstraintUI testbox](images/unity.gif)

### Dependencies

##### SteamVR
Please install Steam. Unity plugin in repo.

##### Optimization (Gurobi)
For the optimization, we use the [Gurobi Optimizer 8.1](https://www.gurobi.com/). It is free for academic use. Please download and install it. The copy the files **Gurobi81.NET.dll** from the folder *$GUROBI_INSTALL_DIR\win64\bin* into the folder *ComputationalMR\CompMR\Assets\Scrips*.

###### ZED SDK
Unity plugin in repo.

We use a ZED mini camera mounted in front of an HTC Vive Pro to deliver a AR experience (i.e., video pass-through AR). *The Unity project works for AR and VR. Please open the scene MainScene-VR for use in VR.* Please install the [Zed SDK 2.7](https://github.com/stereolabs/zed-unity) with all its dependencies (including Cuda) to use this.

###### Pupil Labs Add-on for HTC Vive Pro
We use the [Pupil Labs VR addon for the HTC Vive Pro](https://pupil-labs.com/products/vr-ar/) for eye tracking. This is needed to run the cognitive load estimation (Index of Pupillary Activity), based on the work of [Duchowski et al. (CHI 2018)](https://dl.acm.org/citation.cfm?id=3173856). Please follow the instructions from [https://github.com/pupil-labs/hmd-eyes](https://github.com/pupil-labs/hmd-eyes) on how to install the Pupil SDK for Unity. The **Pupil Service** has to be started manually.

###### Index of Pupillary Activity

The IPA code is based on the code provide in the paper by [Duchowski et al. (CHI 2018)](https://dl.acm.org/citation.cfm?id=3173856). It can be found in the file *Util\pupil-unity-gaze-server.py*.
Unity starts the local server automatically. In order to enable it, you must set the path to the script. In the scenes, there is an object *GazeController*. Specify the *Path To Gaze Server* so that it points to the file (absolute path).

In order to run the **Python 3.6** script from Unity, the following pip packages must be installed:  
*pip install PyWavelets numpy zmq msgpack keyboard*  

If you do not want to use the cognitive load estimation, simply disable the *GazeController* object.

# Contact Information
For questions or problems please file an issue or contact [david.lindlbauer@inf.ethz.ch](mailto:david.lindlbauer@inf.ethz.ch), and use the Github issues page.

# Citation
If you use this code or data for your own work, please use the following citation:

```commandline
@inproceedings{Lindlbauer:2019,
  author = {Lindlbauer, David and Feit, Anna Maria and Hilliges, Otmar},
  title = {Context-Aware Online Adaptation of Mixed Reality Interfaces},
  booktitle = {Proceedings of the 32nd Annual ACM Symposium on User Interface Software and Technology},
  series = {UIST '19},
  year = {2019},
  isbn = {978-1-4503-6816-2/19/10},
  doi = {10.1145/3332165.3347945},
  location = {New Orleans, LA, USA},
  numpages = {10},
  publisher = {ACM},
  address = {New York, NY, USA},
}
```
