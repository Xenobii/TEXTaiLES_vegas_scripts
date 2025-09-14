using System;
using System.Windows.Forms;
using ScriptPortal.Vegas;

public class EntryPoint
{
    private Vegas vegas;
    public void FromVegas(Vegas myVegas)
    {
        vegas = myVegas;

        try
        {
            AddTextailesLogo();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public void AddTextailesLogo()
    {
        // Create track
        int trackIndex = CreateNewVideoTrack();

        // Add transforms to track
        TrackTransforms(trackIndex);

        // Get Image path
        string scriptFolder = AppDomain.CurrentDomain.BaseDirectory;
        string relativePath = @"Script Menu\scripts\res\Icon-Textailes-Colour-RGB-Ver.png";
        string imagePath = System.IO.Path.Combine(scriptFolder, relativePath);

        // Get media duration
        Timecode projectLength = vegas.Project.Length;
        Timecode start = Timecode.FromSeconds(0);

        // Add Logo to project
        VideoEvent imageEvent = (VideoEvent)AddMedia(vegas.Project, imagePath, trackIndex, start, projectLength);
    }

    public int CreateNewVideoTrack()
    {
        // Create track
        VideoTrack track = new VideoTrack();
        vegas.Project.Tracks.Add(track);

        int trackIndex = track.Index;
        return trackIndex;
    }

    public TrackEvent AddMedia(Project project, string mediaPath, int trackIndex, Timecode start, Timecode length)
    {
        Media media = Media.CreateInstance(project, mediaPath);
        Track track = project.Tracks[trackIndex];

        if (track.MediaType == MediaType.Video)
        {
            VideoTrack videoTrack = (VideoTrack)track;

            VideoEvent videoEvent = videoTrack.AddVideoEvent(start, length);

            Take take = videoEvent.AddTake(media.GetVideoStreamByIndex(0));
            return videoEvent;
        }

        return null;
    }

    public void TrackTransforms(int trackIndex)
    {
        // Get track
        Track track = vegas.Project.Tracks[trackIndex];
        if (track == null || track.IsVideo() == false) return;

        VideoTrack videoTrack = (VideoTrack)track;

        // Change composite mode
        videoTrack.CompositeMode = CompositeMode.Dodge;

        // Get keyframe at start
        TrackMotionKeyframe zeroKF = videoTrack.TrackMotion.MotionKeyframes[0];

        // Apply transforms to keyframe
        double width = zeroKF.Width;
        double height = zeroKF.Height;

        zeroKF.Width        = width / 8;
        zeroKF.Height       = height / 8;
        zeroKF.PositionX    = width / 2 - width / 16;
        zeroKF.PositionY    = -height / 2 + height / 8; 
    }
}