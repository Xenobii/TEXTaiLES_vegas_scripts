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
            Smooth();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public void Smooth()
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

            foreach (TrackEvent evnt in videoTrack.Events)
            {
                VideoEvent vEvent = (VideoEvent)evnt;

                foreach (VideoMotionKeyframe vmkf in vEvent.VideoMotion.Keyframes)
                {
                    vmkf.Type = VideoKeyframeType.Smooth;
                }
            }
        }
    }
}