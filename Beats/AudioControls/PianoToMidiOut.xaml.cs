using Beats.Midi;
using System;
using System.Collections.Generic;
using Windows.Devices.Midi;

namespace Beats.AudioControls {
    public sealed partial class PianoToMidiOut {

        public PianoToMidiOut() {
            InitializeComponent();
        }

        private static Dictionary<string, byte> NoteToMidi = new Dictionary<string, byte> {
            {"C", 60},
            {"Cs", 61},
            {"D", 62},
            {"Ds", 63},
            {"E", 64},
            {"F", 65},
            {"Fs", 66},
            {"G", 67},
            {"Gs", 68},
            {"A", 69},
            {"As", 70},
            {"B", 71},
            
        };

        private void PianoControl_OnKeyPressedEvent(object sender, EventArgs e) {
            MidiSelector.SendOut(0, NoteToMidi[(sender as AudioButton).Name], 127, MidiMessageType.NoteOn);
        }

        private void PianoControl_OnKeyReleasedEvent(object sender, EventArgs e) {
            MidiSelector.SendOut(0, NoteToMidi[(sender as AudioButton).Name], 127, MidiMessageType.NoteOff);
        }

    }
}
