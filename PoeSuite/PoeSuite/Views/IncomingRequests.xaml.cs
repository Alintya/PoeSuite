using GalaSoft.MvvmLight.Messaging;
using PoeSuite.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PoeSuite.Views
{
    /// <summary>
    /// Interaction logic for IncomingRequests.xaml
    /// </summary>
    public partial class IncomingRequests : UserControl
    {
        private double _relativeX;
        private double _relativeY;
        private Point _startPoint;
        private bool _firstClick = true;

        public bool IsMoving { get; private set; }


        public IncomingRequests()
        {
            InitializeComponent();
        }

        private void IncomingRequests_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_firstClick)
            {
                GeneralTransform transform = this.TransformToAncestor(this.Parent as Visual);
                _startPoint = transform.Transform(new Point(0 - Properties.Settings.Default.IncomingOverlayX, 0 - Properties.Settings.Default.IncomingOverlayY));
                _firstClick = false;
            }

            Point RelativeMousePoint = Mouse.GetPosition(this);
            _relativeX = RelativeMousePoint.X;
            _relativeY = RelativeMousePoint.Y;

            IsMoving = true;
        }

        private void IncomingRequests_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMoving)
            {
                //Get the position of the mouse relative to the controls parent              
                Point MousePoint = Mouse.GetPosition(this.Parent as IInputElement);
                //set the distance from the original position
                var distanceFromStartX = (int)(MousePoint.X - _startPoint.X - _relativeX);
                var distanceFromStartY = (int)(MousePoint.Y - _startPoint.Y - _relativeY);
                //Set the X and Y coordinates of the RenderTransform to be the distance from original position. This will move the control
                TranslateTransform MoveTransform = base.RenderTransform as TranslateTransform;
                MoveTransform.X = distanceFromStartX;
                MoveTransform.Y = distanceFromStartY;
            }
        }

        private void IncomingRequests_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsMoving = false;
        }
    }
}

