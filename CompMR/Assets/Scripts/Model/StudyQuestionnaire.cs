using System;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class Question
    {
        public string TargetApplication;
        public string QuestionString;
        public string Answer;
        public int AnswerLOD;
    }

    public class StudyQuestionnaire
    {
        public static Question[] Questions =
        {
            new Question
            {
                TargetApplication = "Skype",
                QuestionString = "How many unread messages do you have?",
                Answer = "2",
                AnswerLOD = 1
            },
            new Question
            {
                TargetApplication = "Skype",
                QuestionString = "How many Skype contacts do you have?",
                Answer = "3",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "Skype",
                QuestionString = "What is the name of the second Skype contact?",
                Answer = "Frank, Sam, Christoph",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "Email",
                QuestionString = "How many unread emails do you have?",
                Answer = "",
                AnswerLOD = -1
            },
            new Question
            {
                TargetApplication = "Email",
                QuestionString = "Who sent the last email?",
                Answer = "",
                AnswerLOD = -1
            },
            new Question
            {
                TargetApplication = "Email",
                QuestionString = "What was the last email about?",
                Answer = "",
                AnswerLOD = -1
            },
            new Question
            {
                TargetApplication = "Email",
                QuestionString = "Who sent the last 3 emails?",
                Answer = "",
                AnswerLOD = -1
            },
            new Question
            {
                TargetApplication = "Bank",
                QuestionString = "What is the account balance?",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "Bank",
                QuestionString = "How many transactions are to review? ",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "Bank",
                QuestionString = "What is the account balance in the first account? ",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "Bank",
                QuestionString = "What is the account balance in the second account? ",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "Bank",
                QuestionString = "How many upcoming transactions are in the Account 1? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "Bank",
                QuestionString = "How many upcoming transactions are in the Account 2? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "Bank",
                QuestionString = "What are the last four digits of the credit card in Account 1? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "Bank",
                QuestionString = "What are the last four digits of the credit card in Account 2? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "Food",
                QuestionString = "How many pending food orders are there? ",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "Food",
                QuestionString = "When does you order arrive? ",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "Food",
                QuestionString = "What food did you order? ",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "Food",
                QuestionString = "Which color have the two icons on the map? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "Calendar",
                QuestionString = "When is your next calendar event? ",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "Calendar",
                QuestionString = "What is your next calendar event? ",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "Calendar",
                QuestionString = "At what time do you have lunch with Sam and Ann? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "Calendar",
                QuestionString = "At what time do you meet Frank? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "Calendar",
                QuestionString = "When do you have the call with the company? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "Calendar",
                QuestionString = "When will you go to the gym?",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "News",
                QuestionString = "When was the last news event? ",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "News",
                QuestionString = "What was the most recent news event about? ",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "News",
                QuestionString = "Why did the football trainer get fired? ",
                Answer = "",
                AnswerLOD = 3
            },
            new Question
            {
                TargetApplication = "News",
                QuestionString = "When was the second-last news event?",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "News",
                QuestionString = "What was the second-last news event about? ",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "News",
                QuestionString = "Why did the museum close? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "News",
                QuestionString = "What does the new algorithm sense? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "Tasks",
                QuestionString = "What is the next task? ",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "Tasks",
                QuestionString = "What do you have to write next according to your task list? ",
                Answer = "",
                AnswerLOD = 3
            },
            new Question
            {
                TargetApplication = "Tasks",
                QuestionString = "What do you have to do after writing? ",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "Tasks",
                QuestionString = "What is the second task? ",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "Tasks",
                QuestionString = "What is the third task? ",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "Tasks",
                QuestionString = "When is the deadline for your first task? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "Tasks",
                QuestionString = "When is the deadline for your second task? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "Tasks",
                QuestionString = "When is the deadline for your third task? ",
                Answer = "",
                AnswerLOD = 5
            },
            new Question
            {
                TargetApplication = "Videocall",
                QuestionString = "What color is the shirt of the person in the video call",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "Videocall",
                QuestionString = "What is the person wearing on the head?",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "Youtube",
                QuestionString = "What do you see in the Youtube video?",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "Maps",
                QuestionString = "Is the park left or right on the map?",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "ImageBrowser",
                QuestionString = "What do you see in image number 1? ",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "ImageBrowser",
                QuestionString = "What do you see in image number 2?",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "ImageBrowser",
                QuestionString = "What do you see in image number 3?",
                Answer = "",
                AnswerLOD = 3
            },
            new Question
            {
                TargetApplication = "ImageBrowser",
                QuestionString = "What do you see in image number 4?",
                Answer = "",
                AnswerLOD = 3
            },
            new Question
            {
                TargetApplication = "ImageBrowser",
                QuestionString = "What do you see in image number 5?",
                Answer = "",
                AnswerLOD = 3
            },
            new Question
            {
                TargetApplication = "ImageBrowser",
                QuestionString = "What do you see in image number 6?",
                Answer = "",
                AnswerLOD = 3
            },
            new Question
            {
                TargetApplication = "ImageBrowser",
                QuestionString = "What do you see in image number 7? ",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "ImageBrowser",
                QuestionString = "What do you see in image number 8?",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "ImageBrowser",
                QuestionString = "What do you see in image number nine?",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "Transport",
                QuestionString = "When is your next train? ",
                Answer = "",
                AnswerLOD = 2
            },
            new Question
            {
                TargetApplication = "Transport",
                QuestionString = "When does the second train leave? ",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "Transport",
                QuestionString = "When does the third train leave? ",
                Answer = "",
                AnswerLOD = 4
            },
            new Question
            {
                TargetApplication = "Transport",
                QuestionString = "When does the fourth train leave? ",
                Answer = "",
                AnswerLOD = 4
            },
        };

    }
}