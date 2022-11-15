using System;
using System.Collections.Generic;
using System.Windows;

namespace SecondBrain.notes;

public class LearncardNote : Note
{
    public string Topic { get; }
    public string Question { get; }
    public string Answer { get; }
    
    public LearncardNote(List<string> tags, DateTime timeCreated, string topic, string question, string answer) : base(NoteType.Learncard, topic, tags, timeCreated)
    {
        Topic = topic;
        Question = question;
        Answer = answer;
    }

    public LearncardNote(DateTime timeCreated, string topic, string question, string answer) : base(NoteType.Learncard, topic, timeCreated)
    {
        Topic = topic;
        Question = question;
        Answer = answer;
    }

    public override void AddDefaultTags()
    {
        base.AddDefaultTags();
        Tags.Add(Question);
    }

    public override void OnClick()
    {
        MessageBox.Show( "Question:\n" + Question, "Learncard: " + Topic);
        MessageBox.Show("Answer:\n" + Answer, "Learncard: " + Topic);
    }

    public override string ToString()
    {
        return Topic + ".lc";
    }
}