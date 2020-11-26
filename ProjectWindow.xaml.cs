using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml;
using Microsoft.WindowsAPICodePack.Shell.Interop;
using NAudio.Gui;
using NAudio.Midi;
using NAudio.Wave;

namespace ReChord_Studio
{
    /// <summary>
    /// Interaction logic for ProjectWindow.xaml
    /// </summary>
    public partial class ProjectWindow : Window
    {
        int nRec;
        int shortenable = 0;
        int shortenable2 = 0;
        static int currentTrack = 4;
        MidiOut midiOut = new MidiOut(0);
        bool[] isP = { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        Color black = Color.FromRgb(0, 0, 0);
        Color white = Color.FromRgb(255, 255, 255);
        Color lightblue = Color.FromRgb(51, 204, 255);
        Label selectedLabel;
        Canvas selectedCanvas;
        bool UpArrow = false;
        bool DownArrow = false;
        static int tuning = 0;
        private WaveFileWriter RecordedAudioWriter = null;
        private WasapiLoopbackCapture CaptureInstance = null;
        int vol = 127;
        Color[] colorArr = { Color.FromRgb(102, 224, 255), Color.FromRgb(255, 77, 77), Color.FromRgb(188, 255, 91) };
        Color[] colorArr2 = { Color.FromRgb(51, 192, 228), Color.FromRgb(245, 53, 53), Color.FromRgb(159, 247, 31) };
        System.Media.SoundPlayer player = new SoundPlayer();
        MediaPlayer mp = new MediaPlayer();
        int pointer = 0;
        int currentVolume = 1;
        String dir = "..\\..\\Recordings";
        ImageBrush ib = new ImageBrush();
        

        public ProjectWindow()
        {
            InitializeComponent();
            FillGrid();
            System.IO.DirectoryInfo di = new DirectoryInfo(dir);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            ib.ImageSource = new BitmapImage(new Uri("..\\..\\img\\slider2.png", UriKind.Relative));
            firstControl.Background = ib;
        }


        private void FillGrid() {
            for (int i = 0; i < 60; i++) {
                for (int j = 0; j < 19; j++) {
                    Rectangle r = new Rectangle();
                    r.MouseLeftButtonDown += RectangleCC;
                    r.StrokeThickness = 0.3;
                    r.Fill = new SolidColorBrush(Color.FromRgb(50, 50, 50));
                    r.Stroke = new SolidColorBrush(white);
                    GridPR.Children.Add(r);
                    Grid.SetRow(r, i);
                    Grid.SetColumn(r, j);
                }
            }
            
        }

        private void RectangleCC(object sender, MouseButtonEventArgs e) {
            Rectangle r = (Rectangle)sender;
            if (r.Fill.ToString().Equals("#FF323232")){ 
                r.Fill = new SolidColorBrush(lightblue);
            }
            else {
                r.Fill = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            }
        }

        private void AddTrack_Click(object sender, MouseButtonEventArgs e)
        {
            RowDefinition rd = new RowDefinition();
            rd.Height = new GridLength(120, GridUnitType.Pixel);
            TrackGrid.RowDefinitions.Add(rd);

            Canvas c2 = WPFObjectCopier.Clone<Canvas>(CanvasTrack);
            Canvas c3 = (Canvas)c2.Children[0];
            Label l = (Label)c3.Children[0];
            l.Content = "Track " + currentTrack;
            TrackGrid.Children.Add(c2);
            c2.Background = new SolidColorBrush(colorArr[pointer]);
            c2.PreviewMouseLeftButtonDown += trackhi;
            c3.Background = new SolidColorBrush(colorArr2[pointer]);
            if (pointer == 2)
            {
                pointer = 0;
            }
            else {
                pointer++;
            }
            Grid.SetRow(c2, currentTrack - 1);
            currentTrack++;
            sv.ScrollToBottom();
        }

        public static class WPFObjectCopier
        {
            public static T Clone<T>(T source)
            {
                string objXaml = XamlWriter.Save(source);
                StringReader stringReader = new StringReader(objXaml);
                XmlReader xmlReader = XmlReader.Create(stringReader);
                T t = (T)XamlReader.Load(xmlReader);
                return t;
            }
        }

        private void CButton_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(60 + tuning, vol, 1).RawData);
            Flicker(ABlock, black, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(60 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(60 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(60 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(60 + tuning + 7, vol, 1).RawData);
            }


        }
        private void CButton_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(60 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(60 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(60 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(60 + tuning + 7, 0, 1).RawData);
        }

        private void DButton_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(62 + tuning, vol, 1).RawData);
            Flicker(SBlock, black, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(62 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(62 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(62 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(62 + tuning + 7, vol, 1).RawData);
            }
        }

        private void DButton_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(62 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(62 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(62 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(62 + tuning + 7, 0, 1).RawData);
        }

        private void EButton_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(64 + tuning, vol, 1).RawData);
            Flicker(DBlock, black, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(64 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(64 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(64 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(64 + tuning + 7, vol, 1).RawData);
            }
        }

        private void EButton_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(64 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(64 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(64 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(64 + tuning + 7, 0, 1).RawData);
        }

        private void FButton_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(65 + tuning, vol, 1).RawData);
            Flicker(FBlock, black, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(65 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(65 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(65 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(65 + tuning + 7, vol, 1).RawData);
            }
        }

        private void FButton_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(65 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(65 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(65 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(65 + tuning + 7, 0, 1).RawData);
        }

        private void GButton_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(67 + tuning, vol, 1).RawData);
            Flicker(GBlock, black, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(67 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(67 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(67 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(67 + tuning + 7, vol, 1).RawData);
            }
        }

        private void GButton_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(67 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(67 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(67 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(67 + tuning + 7, 0, 1).RawData);
        }

        private void AButton_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(69 + tuning, vol, 1).RawData);
            Flicker(HBlock, black, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(69 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(69 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(69 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(69 + tuning + 7, vol, 1).RawData);
            }
        }

        private void AButton_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(69 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(69 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(69 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(69 + tuning + 7, 0, 1).RawData);
        }

        private void BButton_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(71 + tuning, vol, 1).RawData);
            Flicker(JBlock, black, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(71 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(71 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(71 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(71 + tuning + 7, vol, 1).RawData);
            }
        }

        private void BButton_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(71 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(71 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(71 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(71 + tuning + 7, 0, 1).RawData);
        }

        private void C2Button_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(72 + tuning, vol, 1).RawData);
            Flicker(KBlock, black, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(72 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(72 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(72 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(72 + tuning + 7, vol, 1).RawData);
            }
        }

        private void C2Button_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(72 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(72 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(72 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(72 + tuning + 7, 0, 1).RawData);
        }

        private void D2Button_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(74 + tuning, vol, 1).RawData);
            Flicker(LBlock, black, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(74 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(74 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(74 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(74 + tuning + 7, vol, 1).RawData);
            }
        }

        private void D2Button_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(74 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(74 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(74 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(74 + tuning + 7, 0, 1).RawData);
        }

        private void E2Button_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(76 + tuning, vol, 1).RawData);
            Flicker(ÇBlock, black, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(76 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(76 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(76 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(76 + tuning + 7, vol, 1).RawData);
            }
        }

        private void E2Button_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(76 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(76 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(76 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(76 + tuning + 7, 0, 1).RawData);
        }

        private void BButton1_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(61 + tuning, vol, 1).RawData);
            Flicker(WBlock, white, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(61 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(61 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(61 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(61 + tuning + 7, vol, 1).RawData);
            }
        }

        private void BButton1_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(61 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(61 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(61 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(61 + tuning + 7, 0, 1).RawData);
        }

        private void BButton2_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(63 + tuning, vol, 1).RawData);
            Flicker(EBlock, white, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(63 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(63 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(63 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(63 + tuning + 7, vol, 1).RawData);
            }
        }

        private void BButton2_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(63 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(63 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(63 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(63 + tuning + 7, 0, 1).RawData);
        }

        private void BButton3_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(66 + tuning, vol, 1).RawData);
            Flicker(TBlock, white, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(66 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(66 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(66 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(66 + tuning + 7, vol, 1).RawData);
            }
        }

        private void BButton3_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(66 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(66 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(66 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(66 + tuning + 7, 0, 1).RawData);
        }

        private void BButton4_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(68 + tuning, vol, 1).RawData);
            Flicker(YBlock, white, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(68 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(68 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(68 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(68 + tuning + 7, vol, 1).RawData);
            }
        }

        private void BButton4_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(68 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(68 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(68 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(68 + tuning + 7, 0, 1).RawData);
        }

        private void BButton5_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(70 + tuning, vol, 1).RawData);
            Flicker(UBlock, white, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(70 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(70 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(70 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(70 + tuning + 7, vol, 1).RawData);
            }
        }

        private void BButton5_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(70 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(70 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(70 + tuning + 3, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(70 + tuning + 7, 0, 1).RawData);
        }

        private void BButton6_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(73 + tuning, vol, 1).RawData);
            Flicker(OBlock, white, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(73 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(73 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(73 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(73 + tuning + 7, vol, 1).RawData);
            }
        }

        private void BButton6_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(73 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(73 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(73 + tuning + 7, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(73 + tuning + 3, 0, 1).RawData);
        }

        private void BButton7_Down(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StartNote(75 + tuning, vol, 1).RawData);
            Flicker(PBlock, white, lightblue);
            if (UpArrow)
            {
                midiOut.Send(MidiMessage.StartNote(75 + tuning + 4, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(75 + tuning + 7, vol, 1).RawData);
            }
            else if (DownArrow)
            {
                midiOut.Send(MidiMessage.StartNote(75 + tuning + 3, vol, 1).RawData);
                midiOut.Send(MidiMessage.StartNote(75 + tuning + 7, vol, 1).RawData);
            }
        }

        private void BButton7_Up(object sender, MouseButtonEventArgs e)
        {
            midiOut.Send(MidiMessage.StopNote(75 + tuning, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(75 + tuning + 4, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(75 + tuning + 7, 0, 1).RawData);
            midiOut.Send(MidiMessage.StopNote(75 + tuning + 3, 0, 1).RawData);
        }

        private void Key_Down(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {

                case (Key.M):
                    UpArrow = true;
                    DownArrow = false;
                    M.Foreground = new SolidColorBrush(lightblue);
                    break;
                case (Key.N):
                    DownArrow = true;
                    UpArrow = false;
                    m.Foreground = new SolidColorBrush(lightblue);
                    break;
                case (Key.A):
                    if (!isP[0])
                    {
                        CButton_Down(sender, null);
                        isP[0] = true;
                    }
                    break;
                case (Key.W):
                    if (!isP[1])
                    {
                        BButton1_Down(sender, null);
                        isP[1] = true;
                    }
                    break;
                case (Key.S):
                    if (!isP[2])
                    {
                        DButton_Down(sender, null);
                        isP[2] = true;
                    }
                    break;
                case (Key.E):
                    if (!isP[3])
                    {
                        BButton2_Down(sender, null);
                        isP[3] = true;
                    }
                    break;
                case (Key.D):
                    if (!isP[4])
                    {
                        EButton_Down(sender, null);
                        isP[4] = true;
                    }
                    break;
                case (Key.F):
                    if (!isP[5])
                    {
                        FButton_Down(sender, null);
                        isP[5] = true;
                    }
                    break;
                case (Key.T):
                    if (!isP[6])
                    {
                        BButton3_Down(sender, null);
                        isP[6] = true;
                    }
                    break;
                case (Key.G):
                    if (!isP[7])
                    {
                        GButton_Down(sender, null);
                        isP[7] = true;
                    }
                    break;
                case (Key.Y):
                    if (!isP[8])
                    {
                        BButton4_Down(sender, null);
                        isP[8] = true;
                    }
                    break;
                case (Key.H):
                    if (!isP[9])
                    {
                        AButton_Down(sender, null);
                        isP[9] = true;
                    }
                    break;
                case (Key.U):
                    if (!isP[10])
                    {
                        BButton5_Down(sender, null);
                        isP[10] = true;
                    }
                    break;
                case (Key.J):
                    if (!isP[11])
                    {
                        BButton_Down(sender, null);
                        isP[11] = true;
                    }
                    break;
                case (Key.K):
                    if (!isP[12])
                    {
                        C2Button_Down(sender, null);
                        isP[12] = true;
                    }
                    break;
                case (Key.O):
                    if (!isP[13])
                    {
                        BButton6_Down(sender, null);
                        isP[13] = true;
                    }
                    break;
                case (Key.L):
                    if (!isP[14])
                    {
                        D2Button_Down(sender, null);
                        isP[14] = true;
                    }
                    break;
                case (Key.P):
                    if (!isP[15])
                    {
                        BButton7_Down(sender, null);
                        isP[15] = true;
                    }
                    break;
                case (Key.Oem3):
                    if (!isP[16])
                    {
                        E2Button_Down(sender, null);
                        isP[16] = true;
                    }
                    break;



            }
        }

        private void Key_Up(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case (Key.M):
                    UpArrow = false;
                    M.Foreground = new SolidColorBrush(white);
                    break;
                case (Key.N):
                    DownArrow = false;
                    m.Foreground = new SolidColorBrush(white);
                    break;
                case (Key.A):
                    CButton_Up(sender, null);
                    isP[0] = false;
                    break;
                case (Key.W):
                    BButton1_Up(sender, null);
                    isP[1] = false;
                    break;
                case (Key.S):
                    DButton_Up(sender, null);
                    isP[2] = false;
                    break;
                case (Key.E):
                    BButton2_Up(sender, null);
                    isP[3] = false;
                    break;
                case (Key.D):
                    EButton_Up(sender, null);
                    isP[4] = false;
                    break;
                case (Key.F):
                    FButton_Up(sender, null);
                    isP[5] = false;
                    break;
                case (Key.T):
                    BButton3_Up(sender, null);
                    isP[6] = false;
                    break;
                case (Key.G):
                    GButton_Up(sender, null);
                    isP[7] = false;
                    break;
                case (Key.Y):
                    BButton4_Up(sender, null);
                    isP[8] = false;
                    break;
                case (Key.H):
                    AButton_Up(sender, null);
                    isP[9] = false;
                    break;
                case (Key.U):
                    BButton5_Up(sender, null);
                    isP[10] = false;
                    break;
                case (Key.J):
                    BButton_Up(sender, null);
                    isP[11] = false;
                    break;
                case (Key.K):
                    C2Button_Up(sender, null);
                    isP[12] = false;
                    break;
                case (Key.O):
                    BButton6_Up(sender, null);
                    isP[13] = false;
                    break;
                case (Key.L):
                    D2Button_Up(sender, null);
                    isP[14] = false;
                    break;
                case (Key.P):
                    BButton7_Up(sender, null);
                    isP[15] = false;
                    break;
                case (Key.Oem3):
                    E2Button_Up(sender, null);
                    isP[16] = false;
                    break;
            }
        }

        private void Flicker(TextBlock tb, Color from, Color to)
        {

            ColorAnimation ca = new ColorAnimation(from, to, TimeSpan.FromSeconds(0.25));
            ca.AutoReverse = true;

            SolidColorBrush solidColorBrush = new SolidColorBrush(Colors.Black);
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca);

            TextEffect textEffect = new TextEffect();
            textEffect.Foreground = solidColorBrush;

            textEffect.PositionStart = 0;
            textEffect.PositionCount = 1;

            tb.TextEffects = new TextEffectCollection();
            tb.TextEffects.Add(textEffect);

        }

        private void ChangeInstFunct(String h) {
            switch (h)
            {
                case ("Acoustic Grand Piano"):
                    midiOut.Send(MidiMessage.ChangePatch(0, 1).RawData);
                    break;
                case ("Bright Acoustic Piano"):
                    midiOut.Send(MidiMessage.ChangePatch(1, 1).RawData);
                    break;
                case ("Electric Grand Piano"):
                    midiOut.Send(MidiMessage.ChangePatch(2, 1).RawData);
                    break;
                case ("Honky-tonk Piano"):
                    midiOut.Send(MidiMessage.ChangePatch(3, 1).RawData);
                    break;
                case ("Electric Piano 1"):
                    midiOut.Send(MidiMessage.ChangePatch(4, 1).RawData);
                    break;
                case ("Electric Piano 2"):
                    midiOut.Send(MidiMessage.ChangePatch(5, 1).RawData);
                    break;
                case ("Harpsichord"):
                    midiOut.Send(MidiMessage.ChangePatch(6, 1).RawData);
                    break;
                case ("Clavi"):
                    midiOut.Send(MidiMessage.ChangePatch(7, 1).RawData);
                    break;
                case ("Celesta"):
                    midiOut.Send(MidiMessage.ChangePatch(8, 1).RawData);
                    break;
                case ("Glockenspiel"):
                    midiOut.Send(MidiMessage.ChangePatch(9, 1).RawData);
                    break;
                case ("Music Box"):
                    midiOut.Send(MidiMessage.ChangePatch(10, 1).RawData);
                    break;
                case ("Vibraphone"):
                    midiOut.Send(MidiMessage.ChangePatch(11, 1).RawData);
                    break;
                case ("Marimba"):
                    midiOut.Send(MidiMessage.ChangePatch(12, 1).RawData);
                    break;
                case ("Xylophone"):
                    midiOut.Send(MidiMessage.ChangePatch(13, 1).RawData);
                    break;
                case ("Tubular Bells"):
                    midiOut.Send(MidiMessage.ChangePatch(14, 1).RawData);
                    break;
                case ("Dulcimer"):
                    midiOut.Send(MidiMessage.ChangePatch(15, 1).RawData);
                    break;
                case ("Drawbar Organ"):
                    midiOut.Send(MidiMessage.ChangePatch(16, 1).RawData);
                    break;
                case ("Percussive Organ"):
                    midiOut.Send(MidiMessage.ChangePatch(17, 1).RawData);
                    break;
                case ("Rock Organ"):
                    midiOut.Send(MidiMessage.ChangePatch(18, 1).RawData);
                    break;
                case ("Church Organ"):
                    midiOut.Send(MidiMessage.ChangePatch(19, 1).RawData);
                    break;
                case ("Reed Organ"):
                    midiOut.Send(MidiMessage.ChangePatch(20, 1).RawData);
                    break;
                case ("Accordion"):
                    midiOut.Send(MidiMessage.ChangePatch(21, 1).RawData);
                    break;
                case ("Harmonica"):
                    midiOut.Send(MidiMessage.ChangePatch(22, 1).RawData);
                    break;
                case ("Tango Accordion"):
                    midiOut.Send(MidiMessage.ChangePatch(23, 1).RawData);
                    break;
                case ("Acoustic Guitar (nylon)"):
                    midiOut.Send(MidiMessage.ChangePatch(24, 1).RawData);
                    break;
                case ("Acoustic Guitar (steel)"):
                    midiOut.Send(MidiMessage.ChangePatch(25, 1).RawData);
                    break;
                case ("Electric Guitar (jazz)"):
                    midiOut.Send(MidiMessage.ChangePatch(26, 1).RawData);
                    break;
                case ("Electric Guitar (clean)"):
                    midiOut.Send(MidiMessage.ChangePatch(27, 1).RawData);
                    break;
                case ("Electric Guitar (muted)"):
                    midiOut.Send(MidiMessage.ChangePatch(28, 1).RawData);
                    break;
                case ("Overdriven Guitar"):
                    midiOut.Send(MidiMessage.ChangePatch(29, 1).RawData);
                    break;
                case ("Distortion Guitar"):
                    midiOut.Send(MidiMessage.ChangePatch(30, 1).RawData);
                    break;
                case ("Guitar Harmonics"):
                    midiOut.Send(MidiMessage.ChangePatch(31, 1).RawData);
                    break;
                case ("Acoustic Bass"):
                    midiOut.Send(MidiMessage.ChangePatch(32, 1).RawData);
                    break;
                case ("Electric Bass (finger)"):
                    midiOut.Send(MidiMessage.ChangePatch(33, 1).RawData);
                    break;
                case ("Electric Bass (pick)"):
                    midiOut.Send(MidiMessage.ChangePatch(34, 1).RawData);
                    break;
                case ("Fretless Bass"):
                    midiOut.Send(MidiMessage.ChangePatch(35, 1).RawData);
                    break;
                case ("Slap Bass 1"):
                    midiOut.Send(MidiMessage.ChangePatch(36, 1).RawData);
                    break;
                case ("Slap Bass 2"):
                    midiOut.Send(MidiMessage.ChangePatch(37, 1).RawData);
                    break;
                case ("Synth Bass 1"):
                    midiOut.Send(MidiMessage.ChangePatch(38, 1).RawData);
                    break;
                case ("Synth Bass 2"):
                    midiOut.Send(MidiMessage.ChangePatch(39, 1).RawData);
                    break;
                case ("Violin"):
                    midiOut.Send(MidiMessage.ChangePatch(40, 1).RawData);
                    break;
                case ("Viola"):
                    midiOut.Send(MidiMessage.ChangePatch(41, 1).RawData);
                    break;
                case ("Cello"):
                    midiOut.Send(MidiMessage.ChangePatch(42, 1).RawData);
                    break;
                case ("Contrabass"):
                    midiOut.Send(MidiMessage.ChangePatch(43, 1).RawData);
                    break;
                case ("Tremolo Strings"):
                    midiOut.Send(MidiMessage.ChangePatch(44, 1).RawData);
                    break;
                case ("Pizzicato Strings"):
                    midiOut.Send(MidiMessage.ChangePatch(45, 1).RawData);
                    break;
                case ("Orchestral Harp"):
                    midiOut.Send(MidiMessage.ChangePatch(46, 1).RawData);
                    break;
                case ("Timpani"):
                    midiOut.Send(MidiMessage.ChangePatch(47, 1).RawData);
                    break;
                case ("String Ensemble 1"):
                    midiOut.Send(MidiMessage.ChangePatch(48, 1).RawData);
                    break;
                case ("String Ensemble 2"):
                    midiOut.Send(MidiMessage.ChangePatch(49, 1).RawData);
                    break;
                case ("Synth Strings 1"):
                    midiOut.Send(MidiMessage.ChangePatch(50, 1).RawData);
                    break;
                case ("Synth Strings 2"):
                    midiOut.Send(MidiMessage.ChangePatch(51, 1).RawData);
                    break;
                case ("Choir Aahs"):
                    midiOut.Send(MidiMessage.ChangePatch(52, 1).RawData);
                    break;
                case ("Voice Oohs"):
                    midiOut.Send(MidiMessage.ChangePatch(53, 1).RawData);
                    break;
                case ("Synth Choir"):
                    midiOut.Send(MidiMessage.ChangePatch(54, 1).RawData);
                    break;
                case ("Orchestra Hit"):
                    midiOut.Send(MidiMessage.ChangePatch(55, 1).RawData);
                    break;
                case ("Trumpet"):
                    midiOut.Send(MidiMessage.ChangePatch(56, 1).RawData);
                    break;
                case ("Trombone"):
                    midiOut.Send(MidiMessage.ChangePatch(57, 1).RawData);
                    break;
                case ("Tuba"):
                    midiOut.Send(MidiMessage.ChangePatch(58, 1).RawData);
                    break;
                case ("Muted Trumpet"):
                    midiOut.Send(MidiMessage.ChangePatch(59, 1).RawData);
                    break;
                case ("French Horn"):
                    midiOut.Send(MidiMessage.ChangePatch(60, 1).RawData);
                    break;
                case ("Brass Section"):
                    midiOut.Send(MidiMessage.ChangePatch(61, 1).RawData);
                    break;
                case ("Synth Brass 1"):
                    midiOut.Send(MidiMessage.ChangePatch(62, 1).RawData);
                    break;
                case ("Synth Brass 2"):
                    midiOut.Send(MidiMessage.ChangePatch(63, 1).RawData);
                    break;
                case ("Soprano Sax"):
                    midiOut.Send(MidiMessage.ChangePatch(64, 1).RawData);
                    break;
                case ("Alto Sax"):
                    midiOut.Send(MidiMessage.ChangePatch(65, 1).RawData);
                    break;
                case ("Tenor Sax"):
                    midiOut.Send(MidiMessage.ChangePatch(66, 1).RawData);
                    break;
                case ("Baritone Sax"):
                    midiOut.Send(MidiMessage.ChangePatch(67, 1).RawData);
                    break;
                case ("Oboe"):
                    midiOut.Send(MidiMessage.ChangePatch(68, 1).RawData);
                    break;
                case ("English Horn"):
                    midiOut.Send(MidiMessage.ChangePatch(69, 1).RawData);
                    break;
                case ("Bassoon"):
                    midiOut.Send(MidiMessage.ChangePatch(70, 1).RawData);
                    break;
                case ("Clarinet"):
                    midiOut.Send(MidiMessage.ChangePatch(71, 1).RawData);
                    break;
                case ("Piccolo"):
                    midiOut.Send(MidiMessage.ChangePatch(72, 1).RawData);
                    break;
                case ("Flute"):
                    midiOut.Send(MidiMessage.ChangePatch(73, 1).RawData);
                    break;
                case ("Recorder"):
                    midiOut.Send(MidiMessage.ChangePatch(74, 1).RawData);
                    break;
                case ("Pan Flute"):
                    midiOut.Send(MidiMessage.ChangePatch(75, 1).RawData);
                    break;
                case ("Blown bottle"):
                    midiOut.Send(MidiMessage.ChangePatch(76, 1).RawData);
                    break;
                case ("Shakuhachi"):
                    midiOut.Send(MidiMessage.ChangePatch(77, 1).RawData);
                    break;
                case ("Whistle"):
                    midiOut.Send(MidiMessage.ChangePatch(78, 1).RawData);
                    break;
                case ("Ocarina"):
                    midiOut.Send(MidiMessage.ChangePatch(79, 1).RawData);
                    break;
                case ("Lead 1 (square)"):
                    midiOut.Send(MidiMessage.ChangePatch(80, 1).RawData);
                    break;
                case ("Lead 2 (sawtooth)"):
                    midiOut.Send(MidiMessage.ChangePatch(81, 1).RawData);
                    break;
                case ("Lead 3 (calliope)"):
                    midiOut.Send(MidiMessage.ChangePatch(82, 1).RawData);
                    break;
                case ("Lead 4 (chiff)"):
                    midiOut.Send(MidiMessage.ChangePatch(83, 1).RawData);
                    break;
                case ("Lead 5 (charang)"):
                    midiOut.Send(MidiMessage.ChangePatch(84, 1).RawData);
                    break;
                case ("Lead 6 (voice)"):
                    midiOut.Send(MidiMessage.ChangePatch(85, 1).RawData);
                    break;
                case ("Lead 7 (fifths)"):
                    midiOut.Send(MidiMessage.ChangePatch(86, 1).RawData);
                    break;
                case ("Lead 8 (bass + lead)"):
                    midiOut.Send(MidiMessage.ChangePatch(87, 1).RawData);
                    break;
                case ("Pad 1 (new age)"):
                    midiOut.Send(MidiMessage.ChangePatch(88, 1).RawData);
                    break;
                case ("Pad 2 (warm)"):
                    midiOut.Send(MidiMessage.ChangePatch(89, 1).RawData);
                    break;
                case ("Pad 3 (polysynth)"):
                    midiOut.Send(MidiMessage.ChangePatch(90, 1).RawData);
                    break;
                case ("Pad 4 (choir)"):
                    midiOut.Send(MidiMessage.ChangePatch(91, 1).RawData);
                    break;
                case ("Pad 5 (bowed)"):
                    midiOut.Send(MidiMessage.ChangePatch(92, 1).RawData);
                    break;
                case ("Pad 6 (metallic)"):
                    midiOut.Send(MidiMessage.ChangePatch(93, 1).RawData);
                    break;
                case ("Pad 7 (halo)"):
                    midiOut.Send(MidiMessage.ChangePatch(94, 1).RawData);
                    break;
                case ("Pad 8 (sweep)"):
                    midiOut.Send(MidiMessage.ChangePatch(95, 1).RawData);
                    break;
                case ("97 FX 1 (rain)"):
                    midiOut.Send(MidiMessage.ChangePatch(96, 1).RawData);
                    break;
                case ("FX 2 (soundtrack)"):
                    midiOut.Send(MidiMessage.ChangePatch(97, 1).RawData);
                    break;
                case ("FX 3 (crystal)"):
                    midiOut.Send(MidiMessage.ChangePatch(98, 1).RawData);
                    break;
                case ("FX 4 (atmosphere)"):
                    midiOut.Send(MidiMessage.ChangePatch(99, 1).RawData);
                    break;
                case ("FX 5 (brightness)"):
                    midiOut.Send(MidiMessage.ChangePatch(100, 1).RawData);
                    break;
                case ("FX 6 (goblins)"):
                    midiOut.Send(MidiMessage.ChangePatch(101, 1).RawData);
                    break;
                case ("FX 7 (echoes)"):
                    midiOut.Send(MidiMessage.ChangePatch(102, 1).RawData);
                    break;
                case ("FX 8 (sci-fi)"):
                    midiOut.Send(MidiMessage.ChangePatch(103, 1).RawData);
                    break;
                case ("Sitar"):
                    midiOut.Send(MidiMessage.ChangePatch(104, 1).RawData);
                    break;
                case ("Banjo"):
                    midiOut.Send(MidiMessage.ChangePatch(105, 1).RawData);
                    break;
                case ("Shamisen"):
                    midiOut.Send(MidiMessage.ChangePatch(106, 1).RawData);
                    break;
                case ("Koto"):
                    midiOut.Send(MidiMessage.ChangePatch(107, 1).RawData);
                    break;
                case ("Kalimba"):
                    midiOut.Send(MidiMessage.ChangePatch(108, 1).RawData);
                    break;
                case ("Bagpipe"):
                    midiOut.Send(MidiMessage.ChangePatch(109, 1).RawData);
                    break;
                case ("Fiddle"):
                    midiOut.Send(MidiMessage.ChangePatch(110, 1).RawData);
                    break;
                case ("Shanai"):
                    midiOut.Send(MidiMessage.ChangePatch(111, 1).RawData);
                    break;
                case ("Tinkle Bell"):
                    midiOut.Send(MidiMessage.ChangePatch(112, 1).RawData);
                    break;
                case ("Agogo"):
                    midiOut.Send(MidiMessage.ChangePatch(113, 1).RawData);
                    break;
                case ("Steel Drums"):
                    midiOut.Send(MidiMessage.ChangePatch(114, 1).RawData);
                    break;
                case ("Woodblock"):
                    midiOut.Send(MidiMessage.ChangePatch(115, 1).RawData);
                    break;
                case ("Taiko Drum"):
                    midiOut.Send(MidiMessage.ChangePatch(116, 1).RawData);
                    break;
                case ("Melodic Tom"):
                    midiOut.Send(MidiMessage.ChangePatch(117, 1).RawData);
                    break;
                case ("Synth Drum"):
                    midiOut.Send(MidiMessage.ChangePatch(118, 1).RawData);
                    break;
                case ("Reverse Cymbal"):
                    midiOut.Send(MidiMessage.ChangePatch(119, 1).RawData);
                    break;
                case ("Guitar Fret Noise"):
                    midiOut.Send(MidiMessage.ChangePatch(120, 1).RawData);
                    break;
                case ("Breath Noise"):
                    midiOut.Send(MidiMessage.ChangePatch(121, 1).RawData);
                    break;
                case ("Seashore"):
                    midiOut.Send(MidiMessage.ChangePatch(122, 1).RawData);
                    break;
                case ("Bird Tweet"):
                    midiOut.Send(MidiMessage.ChangePatch(123, 1).RawData);
                    break;
                case ("Telephone Ring"):
                    midiOut.Send(MidiMessage.ChangePatch(124, 1).RawData);
                    break;
                case ("Helicopter"):
                    midiOut.Send(MidiMessage.ChangePatch(125, 1).RawData);
                    break;
                case ("Applause"):
                    midiOut.Send(MidiMessage.ChangePatch(126, 1).RawData);
                    break;
                case ("Gunshot"):
                    midiOut.Send(MidiMessage.ChangePatch(127, 1).RawData);
                    break;
            }
        }

        private void ChangeInstrument(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem tvi = (TreeViewItem)sender;
            String h = tvi.Header.ToString();
            ChangeInstFunct(h);
            
        }

        private void OpenPR(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                PianoRoll.Visibility = Visibility.Visible;
            }
            else
            {
            }
            e.Handled = true;
        }

        private void OpenPR2(object sender, MouseButtonEventArgs e)
        {
            PianoRoll.Visibility = Visibility.Visible;
        }

        private void ClosePR(object sender, MouseButtonEventArgs e)
        {
            PianoRoll.Visibility = Visibility.Hidden;
        }

        private void TuneUpButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (tuning < 43)
            {
                tuning++;
                tune.Content = "Tuning: " + tuning;
            }
        }

        private void TuneDownButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (tuning > -60)
            {
                tuning--;
                tune.Content = "Tuning: " + tuning;
            }
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            instHere.Visibility = Visibility.Visible;
            HitBox.IsHitTestVisible = true;
            TreeViewItem tvi = (TreeViewItem)sender;
            string header = tvi.Header.ToString();
            string dataFormat = DataFormats.UnicodeText;
            DataObject dataObj = new DataObject(dataFormat, header);
            DragDrop.DoDragDrop(tvi, dataObj, DragDropEffects.Copy);
            HitBox.IsHitTestVisible =false;
            instHere.Visibility = Visibility.Hidden;
            
        }

        private void Drag2(object sender, MouseButtonEventArgs e)
        {
            effHere.Visibility = Visibility.Visible;
            HitBox.Visibility = Visibility.Hidden;
            TreeViewItem tvi = (TreeViewItem)sender;
            string header = tvi.Header.ToString();
            string dataFormat = DataFormats.UnicodeText;
            DataObject dataObj = new DataObject(dataFormat, header);
            DragDrop.DoDragDrop(tvi, dataObj, DragDropEffects.Copy);
            HitBox.Visibility = Visibility.Visible;
            effHere.Visibility = Visibility.Hidden;
        }

        private void Drop(object sender, DragEventArgs e)
        {
            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(161, GridUnitType.Pixel);
            VolumeGrid.ColumnDefinitions.Add(cd);

            Border b = new Border();
            Canvas c = new Canvas();
            DockPanel sp = new DockPanel();
            TextBlock tb = new TextBlock();
            Slider s = new Slider();
            ToolTip tt = new ToolTip();
            ContextMenu cm = new ContextMenu();
            MenuItem mi = new MenuItem();
            MenuItem mi2 = new MenuItem();

            b.BorderBrush = new SolidColorBrush(Colors.White);
            b.BorderThickness = new Thickness(1);

            sp.Width = 161;
            sp.Height = 240;

            tb.TextAlignment = TextAlignment.Center;
            tb.FontSize = 18;
            tb.Height = 48;
            

            s.Orientation = Orientation.Vertical;
            s.Value = 127;
            s.Width = 10;
            s.Height = 150;
            s.Minimum = 0;
            s.Maximum = 127;
            s.ValueChanged += SliderChange;

            tb.TextWrapping = TextWrapping.Wrap;
            tb.Text = (string)e.Data.GetData(DataFormats.StringFormat);
            tb.Foreground = new SolidColorBrush(Colors.White);

            sp.Children.Add(tb);
            sp.Children.Add(s);

            tt.Content = "Effects:";

            c.AllowDrop = true;
            c.Drop += Drop2;
            c.ContextMenu = cm;
            cm.PlacementTarget = c;
            c.Background = ib;
            c.MouseLeftButtonDown += ClickStackPanel;
            c.Children.Add(sp);
            c.ToolTip = tt;
            b.Child = c;
            mi.Header = "Remove";
            mi.PreviewMouseLeftButtonDown += closeInst;
            mi2.Header = "Clear Effects";
            mi2.PreviewMouseLeftButtonDown += removeEffects;
            cm.Items.Add(mi);
            cm.Items.Add(mi2);
            VolumeGrid.Children.Add(b);
            DockPanel.SetDock(s, Dock.Bottom);
            DockPanel.SetDock(tb, Dock.Top);
            Grid.SetColumn(b, currentVolume);
            currentVolume++;
            vol = (int)s.Value;
            ChangeInstFunct(tb.Text);
            HitBox.IsHitTestVisible = false;
            instHere.Visibility = Visibility.Hidden;
        }

        private void ClickStackPanel(object sender, MouseButtonEventArgs e)
        {
            Canvas c = (Canvas)sender;
            DockPanel sp = (DockPanel)c.Children[0];
            Slider s = (Slider)sp.Children[1];
            TextBlock tb = (TextBlock)sp.Children[0];
            ChangeInstFunct(tb.Text);
            vol = (int)s.Value;
        }

        private void SliderChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = (Slider)sender;
            DockPanel sp = (DockPanel)s.Parent;
            TextBlock tb = (TextBlock)sp.Children[0];
            ChangeInstFunct(tb.Text);
            vol = (int)s.Value;
        }

        private void record(object sender, MouseButtonEventArgs e)
        {
            if (testPlay())
            {
                RecBut.Visibility = Visibility.Hidden;
                StopBut.Visibility = Visibility.Visible;

                // Define the output wav file of the recorded audio
                string outputFilePath = @"..\\..\\Recordings\Track" + nRec + ".wav";

                // Redefine the capturer instance with a new instance of the LoopbackCapture class
                this.CaptureInstance = new WasapiLoopbackCapture();

                // Redefine the audio writer instance with the given configuration
                this.RecordedAudioWriter = new WaveFileWriter(outputFilePath, CaptureInstance.WaveFormat);

                // When the capturer receives audio, start writing the buffer into the mentioned file
                this.CaptureInstance.DataAvailable += (s, a) =>
                {
                    this.RecordedAudioWriter.Write(a.Buffer, 0, a.BytesRecorded);
                };

                // When the Capturer Stops
                this.CaptureInstance.RecordingStopped += (s, a) =>
                {
                    this.RecordedAudioWriter.Dispose();
                    this.RecordedAudioWriter = null;
                    CaptureInstance.Dispose();
                };

                // Start recording !
                this.CaptureInstance.StartRecording();
            }
            else {
                Error1.Visibility = Visibility.Visible;
            }
        }

        private void error1Hide(object sender, MouseButtonEventArgs e) {
            Error1.Visibility = Visibility.Hidden;  
        }

        private void Stop(object sender, MouseButtonEventArgs e)
        {
            RecBut.Visibility = Visibility.Visible;
            StopBut.Visibility = Visibility.Hidden;
            // Stop recording !
            this.CaptureInstance.StopRecording();
        }

        private void reTune(object sender, MouseButtonEventArgs e)
        {
            tuning = 0;
            tune.Content = "Tuning: 0";
        }

        private void EffectsButton(object sender, MouseButtonEventArgs e)
        {
            InstList.Visibility = Visibility.Hidden;
            EffectList.Visibility = Visibility.Visible;
        }

        private void IntrumentsButton(object sender, MouseButtonEventArgs e)
        {
            InstList.Visibility = Visibility.Visible;
            EffectList.Visibility = Visibility.Hidden;
        }

        private void Drop2(object sender, DragEventArgs e)
        {
            HitBox.Visibility = Visibility.Visible;
            Canvas c = (Canvas)sender;
            String effect = (String)e.Data.GetData(DataFormats.StringFormat);
            if (!c.ToolTip.ToString().Contains(effect))
            {
                c.ToolTip += effect + "/ ";
            }

            if (c.ToolTip.ToString().Contains("System.Windows.Controls.ToolTip: ")) {
                c.ToolTip = c.ToolTip.ToString().Replace("System.Windows.Controls.ToolTip: ", "");
            }
            effHere.Visibility = Visibility.Hidden;
        }

        private void TStep2(object sender, MouseButtonEventArgs e)
        {
            T1.Visibility = Visibility.Hidden;
            T2.Visibility = Visibility.Visible;
            shen.Visibility = Visibility.Hidden;
        }

        private void TStep3(object sender, MouseButtonEventArgs e)
        {
            T2.Visibility = Visibility.Hidden;
            T3.Visibility = Visibility.Visible;
            shen.Visibility = Visibility.Visible;
        }

        private void TStep4(object sender, MouseButtonEventArgs e)
        {
            T3.Visibility = Visibility.Hidden;
            T4.Visibility = Visibility.Visible;
        }

        private void TStep5(object sender, MouseButtonEventArgs e)
        {
            T4.Visibility = Visibility.Hidden;
            T5.Visibility = Visibility.Visible;
            InstList.Visibility = Visibility.Hidden;
            EffectList.Visibility = Visibility.Visible;
        }

        private void TStep6(object sender, MouseButtonEventArgs e)
        {
            T5.Visibility = Visibility.Hidden;
            T6.Visibility = Visibility.Visible;
            InstList.Visibility = Visibility.Visible;
            EffectList.Visibility = Visibility.Hidden;
        }

        private void EndTuto(object sender, MouseButtonEventArgs e)
        {
            T6.Visibility = Visibility.Hidden;
        }

        private void SkipTuto(object sender, MouseButtonEventArgs e)
        {
            T1.Visibility = Visibility.Hidden;
        }

        private void saved(object sender, MouseButtonEventArgs e) {
            MessageBox.Show("Project Saved!");
        }

        private void newproject(object sender, MouseEventArgs e)
        {
            midiOut.Close();
            ProjectWindow pj = new ProjectWindow();
            pj.Show();
            App.Current.Windows[0].Close(); 
        }

        private void enabletut(object sender, MouseEventArgs e)
        {
            T1.Visibility = Visibility.Visible;
        }

        private void shortcut(object sender, MouseEventArgs e)
        {
            if (shortenable == 0)
            {
                shen.Visibility = Visibility.Hidden;
                shortenable = 1;
            }
            else {
                shen.Visibility = Visibility.Visible;
                shortenable = 0;
            }

            
        }


        private void shortcut2(object sender, MouseEventArgs e)
        {
            if (shortenable2 == 0)
            {
                NoteDic.Visibility = Visibility.Hidden;
                shortenable2 = 1;
            }
            else
            {
                NoteDic.Visibility = Visibility.Visible;
                shortenable2 = 0;
            }


        }

        private void trackhi(object sender, MouseEventArgs e)
        {

            Canvas c = (Canvas)sender;
            Canvas c2 = (Canvas)c.Children[0];
            Label l = (Label)c2.Children[0];

            if (nRec != int.Parse(l.Content.ToString().Substring(l.Content.ToString().Length - 1)))
            {

                nRec = int.Parse(l.Content.ToString().Substring(l.Content.ToString().Length - 1));


                if (selectedLabel != null)
                {
                    String mod = selectedLabel.Content.ToString().Substring(1);
                    selectedLabel.Content = mod;
                    selectedCanvas.Opacity = 1;
                    selectedLabel = l;
                    selectedCanvas = c;
                    String original = l.Content.ToString();
                    l.Content = "➔" + original;
                    c.Opacity = 0.7;
                }
                else
                {
                    selectedLabel = l;
                    selectedCanvas = c;
                    String original = l.Content.ToString();
                    l.Content = "➔" + original;
                    selectedCanvas.Opacity = 0.7;
                }
            }
            else {
                String mod = selectedLabel.Content.ToString().Substring(1);
                selectedLabel.Content = mod;
                selectedCanvas.Opacity = 1;
                nRec = 0;
                selectedLabel = null;
                selectedCanvas = null;
            }
            if (nRec == 0)
            {
                selectedTrack.Content = "Selected Track Number: none";
            }
            else {
                selectedTrack.Content = "Selected Track Number: " + nRec;
            }


        }

        private void closeInst(object sender, MouseEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            ContextMenu cm = (ContextMenu)mi.Parent;
            Canvas c = (Canvas)cm.PlacementTarget;
            Border b = (Border)c.Parent;
            int column = VolumeGrid.Children.IndexOf(b);
            for (int i = column+1; i < VolumeGrid.Children.Count; i++) {
                Grid.SetColumn(VolumeGrid.Children[i], i-1);
            }
            VolumeGrid.Children.Remove(b);
            currentVolume = VolumeGrid.Children.Count;
            VolumeGrid.ColumnDefinitions.RemoveAt(currentVolume);
            if (currentVolume > 0)
            {
                Border b1 = (Border)VolumeGrid.Children[0];
                Canvas c1 = (Canvas)b1.Child;
                ClickStackPanel(c1, null);
            }
            else {
                ChangeInstFunct("Acoustic Grand Piano");
                vol = 127;
            }
        }

        private void removeEffects(object sender, MouseEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            ContextMenu cm = (ContextMenu)mi.Parent;
            Canvas c = (Canvas)cm.PlacementTarget;

            c.ToolTip = "Effects: ";
        }

        private void shutdown(object sender, MouseEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Play(object sender, MouseEventArgs e)
        {
            String dir = "..\\..\\Recordings";

            if (testPlay() && File.Exists(dir+"\\Track" + nRec + ".wav") && StopBut.Visibility==Visibility.Hidden)
            {
                dir += "\\Track" + nRec + ".wav"; 
                player = new System.Media.SoundPlayer(dir);
                player.Play();
            }

            if (nRec == 0) {
                System.IO.DirectoryInfo di = new DirectoryInfo(dir);

                foreach (FileInfo file in di.GetFiles())
                {
                    MediaPlayer mp = new MediaPlayer();
                    mp.Open(new System.Uri(di.ToString() + "\\" + file.Name, UriKind.Relative));
                    mp.Play();
                }
            }
        }   

        private void Pause(object sender, MouseEventArgs e)
        {
            if (StopBut.Visibility == Visibility.Hidden) {
                player.Stop();
            }
        }

        private Boolean testPlay() {
            if (nRec > 0) {
                return true;
            }
            return false;
        }
    }
}
