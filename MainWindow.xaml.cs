using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;

namespace ReChord_Studio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MouseClickNew(object sender, MouseButtonEventArgs e)
        {
            PopupNew.Visibility = Visibility.Visible;
            DoubleAnimation animation = new DoubleAnimation(0,1,TimeSpan.FromSeconds(0.25));
            PopupNew.BeginAnimation(Canvas.OpacityProperty, animation);
        }

        private void MouseEnterNew(object sender, MouseEventArgs e)
        {
            NewTB.Cursor = Cursors.Hand;
            NewTB.TextDecorations = TextDecorations.Underline;
        }

        private void MouseLeaveNew(object sender, MouseEventArgs e)
        {
            NewTB.TextDecorations = null;
        }

        private void MouseClickLearn(object sender, MouseButtonEventArgs e)
        {
            MiniGame mg = new MiniGame();
            mg.Show();
            App.Current.Windows[0].Close();
        }

        private void MouseEnterLearn(object sender, MouseEventArgs e)
        {
            LearnTB.Cursor = Cursors.Hand;
            LearnTB.TextDecorations = TextDecorations.Underline;
        }

        private void MouseLeaveLearn(object sender, MouseEventArgs e)
        {
            LearnTB.TextDecorations = null;
        }

        private void CancelButtonClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DoubleAnimation animation = new DoubleAnimation(0, TimeSpan.FromSeconds(0.25));
                PopupNew.BeginAnimation(Canvas.OpacityProperty, animation);
                PopupNew.Visibility = Visibility.Hidden;
            }
        }




        private void ContinueButtonClick(object sender, MouseButtonEventArgs e)
        {
            ProjectWindow pj = new ProjectWindow();
            pj.Show();
            App.Current.Windows[0].Close();
        }

        private void OpenProjectClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Filter = "TXT files(*.txt)|*.txt";

            //Show the dialog to the user
            bool? result = FileDialog.ShowDialog();

            //If the user selected something perform the actions with the file
            if (result.HasValue && result.Value)
            {
            }
        }

        private void OpenProjectEnter(object sender, MouseEventArgs e)
        {
            OpenProject.Cursor = Cursors.Hand;
            OpenProject.TextDecorations = TextDecorations.Underline;
        }

        private void OpenProjectLeave(object sender, MouseEventArgs e)
        {
            OpenProject.TextDecorations = null;
        }

        private void FocusProjectName(object sender, RoutedEventArgs e)
        {
            if (ProjectNameTB.Text == "Project Name")
            {
                ProjectNameTB.Text = null;
            }
        }


        private void LocationClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void SaveDirButtonClick(object sender, MouseButtonEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            string CombinedPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\Projects");
            dialog.InitialDirectory = System.IO.Path.GetFullPath(CombinedPath);
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DestinationTB.Text = dialog.FileName;
            }
        }

        private void ProjectNameLostFocus(object sender, RoutedEventArgs e)
        {
            if (ProjectNameTB.Text == "") {
                ProjectNameTB.Text = "Project Name";
            }
        }
    }
}
