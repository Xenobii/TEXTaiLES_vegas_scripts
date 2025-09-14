using System;
using System.Windows.Forms;
using ScriptPortal.Vegas;
using System.Globalization;
using System.Drawing;

public class EntryPoint
{
    private Vegas vegas;
    public void FromVegas(Vegas myVegas)
    {
        vegas = myVegas;

        try
        {
            int duration = 5;
            AddText(duration);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    public void AddText(int duration)
    {
        // Get top track
        int trackIndex = 0;

        if (vegas.Project.Tracks.Count == 0)
        {
            throw new ArgumentException("Top track doesn't exist");
        }

        Track track = vegas.Project.Tracks[trackIndex];

        if (track.IsVideo() == false)
        {
            throw new ArgumentException("Top track is not a video track");
        }

        VideoTrack videoTrack = (VideoTrack)track;

        // Get text plugin
        string genUID = "{Svfx:com.vegascreativesoftware:titlesandtext}";
        PlugInNode plugin = null;
        plugin = vegas.Generators.GetChildByUniqueID(genUID);

        // Create text media
        Media textMedia = new Media(plugin);
        MediaStream stream = textMedia.Streams.GetItemByMediaType(MediaType.Video, 0);

        // Define start and length
        Timecode start = vegas.Transport.CursorPosition;
        Timecode length = Timecode.FromSeconds(duration);

        // Create text event
        VideoEvent textEvent = new VideoEvent(start, length);
        videoTrack.Events.Add(textEvent);
        Take take = new Take(stream);
        textEvent.Takes.Add(take);

        // Add fade in/out
        AddFadeIn(textEvent, 1);
        AddFadeOut(textEvent, 1);

        // Modify text
        Effect effect = textEvent.ActiveTake.Media.Generator;
        ModifyText(effect);
    }

    public void AddFadeIn(VideoEvent evnt, int fadeInDuration)
    {
        evnt.FadeIn.Length = Timecode.FromSeconds(fadeInDuration);
    }

    public void AddFadeOut(VideoEvent evnt, int fadeOutDuration)
    {
        evnt.FadeOut.Length = Timecode.FromSeconds(fadeOutDuration);
    }

    public void ModifyText(Effect effect)
    {
        OFXEffect fxo = effect.OFXEffect;

        OFXStringParameter text = (OFXStringParameter)fxo.FindParameterByName("Text");
        RichTextBox rtfText = new RichTextBox();

        // Set font and size
        rtfText.Text = "--insert text--";
        rtfText.SelectAll();
        rtfText.SelectionFont = new Font("Times New Roman", 24, FontStyle.Bold);
        text.Value = rtfText.Rtf;

        // Set font color
        OFXRGBAParameter fc = (OFXRGBAParameter)fxo.FindParameterByName("TextColor");
        Color text_color = Color.White;

        OFXColor foc;
        foc.R = text_color.R / 255.0;
        foc.G = text_color.G / 255.0;
        foc.B = text_color.B / 255.0;
        foc.A = text_color.A / 255.0;
        fc.Value = foc;

        // Change location to bottom
        OFXDouble2DParameter position = (OFXDouble2DParameter)fxo.FindParameterByName("Location");
        OFXDouble2D pos;
        pos.X = 0.5;
        pos.Y = 0.2;
        position.Value = pos;

        // Add shadow
        OFXParameter shadow = (OFXParameter)fxo.FindParameterByName("Shadow");
        OFXBooleanParameter enableShadow = (OFXBooleanParameter)fxo.FindParameterByName("ShadowEnable");
        enableShadow.Value = true;

        // Edit Shadow 
        OFXDoubleParameter shadowOffsetX = (OFXDoubleParameter)fxo.FindParameterByName("ShadowOffsetX");
        OFXDoubleParameter shadowOffsetY = (OFXDoubleParameter)fxo.FindParameterByName("ShadowOffsetY");
        shadowOffsetX.Value = 0.0;
        shadowOffsetY.Value = 0.1;

        // Apply changes
        fxo.AllParametersChanged();
    }
}