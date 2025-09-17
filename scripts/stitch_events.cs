using System;
using ScriptPortal.Vegas;
using System.Windows.Forms;


public class EntryPoint
{
    private Vegas vegas;
    public void FromVegas(Vegas myVegas)
    {
        vegas = myVegas;

        try
        {
            Stitch();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public void Stitch()
    {
        if (vegas.Project.Tracks.Count == 0)
        {
            throw new ArgumentException("No tracks in this project");
        }

        foreach (Track track in vegas.Project.Tracks)
        {
            // Iterate over selected tracks
            if (!track.Selected) continue;

            VideoTrack videoTrack = (VideoTrack)track;
            if (videoTrack == null) continue;

            // Sort events by start time
            TrackEvent[] events = new TrackEvent[videoTrack.Events.Count];
            videoTrack.Events.CopyTo(events, 0);
            Array.Sort(events, (a, b) => a.Start.CompareTo(b.Start));

            Timecode start = events[0].Start;

            foreach (TrackEvent ev in events)
            {
                ev.Start = start;
                start = ev.End;
            }

        }
    }
}