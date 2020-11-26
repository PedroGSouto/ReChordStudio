using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NAudio.Gui;
using NAudio.Midi;
using NAudio.Wave;

namespace ReChord_Studio
{

    public partial class MiniGame : Window
    {
        MidiOut midiOut = new MidiOut(0);
        public MiniGame()
        {
            InitializeComponent();
        }

        private void TheoryC(object sender, MouseButtonEventArgs e)
        {
            Menu.Visibility = Visibility.Hidden;
            Teo1.Visibility = Visibility.Visible;
        }

        private void TheoryK(object sender, MouseButtonEventArgs e)
        {
            Menu.Visibility = Visibility.Hidden;
            Teo2.Visibility = Visibility.Visible;
        }

        private void MG11(object sender, MouseButtonEventArgs e)
        {
            Menu.Visibility = Visibility.Hidden;
            MG1.Visibility = Visibility.Visible;
        }

        private void Mg2(object sender, MouseButtonEventArgs e)
        {
            Menu.Visibility = Visibility.Hidden;
            MG2.Visibility = Visibility.Visible;
        }

        private void close(object sender, MouseButtonEventArgs e)
        {
            Image im = (Image)sender;
            Canvas c = (Canvas)im.Parent;
            c.Visibility = Visibility.Hidden;
            Menu.Visibility = Visibility.Visible;
        }

        private void MouseEnterE(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Cursor = Cursors.Hand;
            tb.TextDecorations = TextDecorations.Underline;
        }

        private void MouseLeaveL(object sender, MouseEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            tb.Cursor = Cursors.Hand;
            tb.TextDecorations = null;
        }

        private void MG1Over(object sender, MouseEventArgs e)
        {
            RA.Background = new SolidColorBrush(Colors.Green);
            WA.Background = new SolidColorBrush(Colors.Red);
            WA2.Background = new SolidColorBrush(Colors.Red);
            WA3.Background = new SolidColorBrush(Colors.Red);
        }

        private void MG2Over(object sender, MouseEventArgs e)
        {
            RAA.Background = new SolidColorBrush(Colors.Green);
            WAA.Background = new SolidColorBrush(Colors.Red);
            WAA2.Background = new SolidColorBrush(Colors.Red);
            WAA3.Background = new SolidColorBrush(Colors.Red);
        }
        private void playsound1(object sender, MouseEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(65, 127, 1).RawData);
            midiOut.Send(MidiMessage.StartNote(69, 127, 1).RawData);
            midiOut.Send(MidiMessage.StartNote(72, 127, 1).RawData);
        }

        private void playsound2(object sender, MouseEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(60, 127, 1).RawData);
            midiOut.Send(MidiMessage.StartNote(65, 127, 1).RawData);
        }

        private void playsoundC(object sender, MouseEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(60, 127, 1).RawData);
        }

        private void menter(object sender, MouseEventArgs e)
        {
            Image im = (Image)sender;
            im.Cursor = Cursors.Hand;
        }

        private void mleave(object sender, MouseEventArgs e)
        {
            Image im = (Image)sender;
            im.Cursor = Cursors.Hand;
        }



    }
}
