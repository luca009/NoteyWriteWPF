using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace NoteyWriteWPF
{
    class imageResizeAdorner : Adorner
    {
        Thumb topLeft, topRight, bottomLeft, bottomRight;
        Button alignLeft, alignCenter, alignRight, cancel, stretch, fit;
        VisualCollection visualChildren;

        public imageResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            visualChildren = new VisualCollection(this);
            MainWindow mainWindow = new MainWindow();
            // Call a helper method to initialize the Thumbs 
            // with a customized cursors. 
            //BuildAdornerCorner(ref topLeft, Cursors.SizeNWSE);
            //BuildAdornerCorner(ref topRight, Cursors.SizeNESW);
            //BuildAdornerCorner(ref bottomLeft, Cursors.SizeNESW);
            BuildAdornerCorner(ref bottomRight, Cursors.SizeNWSE);
            alignLeft = new Button() { Content = mainWindow.Resources["iconAlignLeft"], Width = 20, Height = 20 };
            alignCenter = new Button() { Content = mainWindow.Resources["iconAlignCenter"], Width = 20, Height = 20 };
            alignRight = new Button() { Content = mainWindow.Resources["iconAlignRight"], Width = 20, Height = 20 };
            cancel = new Button() { Content = "X", Width = 20, Height = 20 };
            stretch = new Button() { Content = mainWindow.Resources["iconStretch"], Width = 20, Height = 20 };
            fit = new Button() { Content = mainWindow.Resources["iconFit"], Width = 20, Height = 20 };
            visualChildren.Add(alignLeft);
            visualChildren.Add(alignCenter);
            visualChildren.Add(alignRight);
            visualChildren.Add(cancel);
            visualChildren.Add(stretch);
            visualChildren.Add(fit);

            // Add handlers for resizing. 
            //bottomLeft.DragDelta += new DragDeltaEventHandler(HandleBottomLeft);
            bottomRight.DragDelta += new DragDeltaEventHandler(HandleBottomRight);
            //topLeft.DragDelta += new DragDeltaEventHandler(HandleTopLeft);
            //topRight.DragDelta += new DragDeltaEventHandler(HandleTopRight);
            alignLeft.Click += new RoutedEventHandler(alignLeft_Click);
            alignCenter.Click += new RoutedEventHandler(alignCenter_Click);
            alignRight.Click += new RoutedEventHandler(alignRight_Click);
            cancel.Click += new RoutedEventHandler(cancel_Click);
            stretch.Click += new RoutedEventHandler(stretch_Click);
            fit.Click += new RoutedEventHandler(fit_Click);
        }

        /*protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);

            // Some arbitrary drawing implements.
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            double renderRadius = 5.0;

            // Draw a circle at each corner.
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
        }*/

        void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize. 
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger  
            // than the width or height of an adorner, respectively. 
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the bottom-left. 
        void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize. 
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger  
            // than the width or height of an adorner, respectively. 
            adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-right. 
        void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;
            FrameworkElement parentElement = adornedElement.Parent as FrameworkElement;

            // Ensure that the Width and Height are properly initialized after the resize. 
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger  
            // than the width or height of an adorner, respectively. 
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-left. 
        void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            FrameworkElement adornedElement = AdornedElement as FrameworkElement;
            Thumb hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize. 
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger  
            // than the width or height of an adorner, respectively. 
            adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
        }

        void alignLeft_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            adornedElement.HorizontalAlignment = HorizontalAlignment.Left;
        }

        void alignCenter_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            adornedElement.HorizontalAlignment = HorizontalAlignment.Center;
        }

        void alignRight_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement adornedElement = this.AdornedElement as FrameworkElement;
            adornedElement.HorizontalAlignment = HorizontalAlignment.Right;
        }

        void cancel_Click(object sender, RoutedEventArgs e)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this.AdornedElement);
            if (adornerLayer != null)
                foreach (Adorner adorner in adornerLayer.GetAdorners(this.AdornedElement))
                    adornerLayer.Remove(adorner);
        }

        private void fit_Click(object sender, RoutedEventArgs e)
        {
            Image adornedElement = this.AdornedElement as Image;
            adornedElement.Stretch = Stretch.Uniform;
        }

        private void stretch_Click(object sender, RoutedEventArgs e)
        {
            Image adornedElement = this.AdornedElement as Image;
            adornedElement.Stretch = Stretch.Fill;
        }

        // Arrange the Adorners. 
        protected override Size ArrangeOverride(Size finalSize)
        {
            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.   
            // These will be used to place the ResizingAdorner at the corners of the adorned element.   
            double desiredWidth = AdornedElement.DesiredSize.Width;
            double desiredHeight = AdornedElement.DesiredSize.Height;
            // adornerWidth & adornerHeight are used for placement as well. 
            double adornerWidth = this.DesiredSize.Width;
            double adornerHeight = this.DesiredSize.Height;

            //topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            alignLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            alignCenter.Arrange(new Rect(-adornerWidth / 2 + 20, -adornerHeight / 2, adornerWidth, adornerHeight));
            alignRight.Arrange(new Rect(-adornerWidth / 2 + 40, -adornerHeight / 2, adornerWidth, adornerHeight));
            stretch.Arrange(new Rect(-adornerWidth / 2 + 70, -adornerHeight / 2, adornerWidth, adornerHeight));
            fit.Arrange(new Rect(-adornerWidth / 2 + 90, -adornerHeight / 2, adornerWidth, adornerHeight));
            cancel.Arrange(new Rect(-adornerWidth / 2 + 120, -adornerHeight / 2, adornerWidth, adornerHeight));
            //topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            //bottomLeft.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));

            // Return the final size. 
            return finalSize;
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property,  
        // set some appearance properties, and add the elements to the visual tree. 
        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics. 
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 10;
            cornerThumb.Opacity = 0.80;
            cornerThumb.Background = new SolidColorBrush(Colors.White);

            visualChildren.Add(cornerThumb);
        }

        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces 
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height 
        // need to be set first.  It also sets the maximum size of the adorned element. 
        void EnforceSize(FrameworkElement adornedElement)
        {
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            FrameworkElement parent = adornedElement.Parent as FrameworkElement;
            if (parent != null)
            {
                adornedElement.MaxHeight = parent.ActualHeight;
                adornedElement.MaxWidth = parent.ActualWidth;
            }
        }
        // Override the VisualChildrenCount and GetVisualChild properties to interface with  
        // the adorner's visual collection. 
        protected override int VisualChildrenCount { get { return visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return visualChildren[index]; }
    }
}
