﻿using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LbmScreenConfig;
using Erp.Mvvm;

namespace LittleBigMouse.ControlCore
{
    public class ViewModeMultiScreenBackgound : ViewMode { }

    /// <summary>
    /// Logique d'interaction pour MultiScreensGui.xaml
    /// </summary>
    public partial class MultiScreensView : UserControl, IView<ViewModeDefault,MultiScreensViewModel>, IViewClassDefault
    {
        public MultiScreensView()
        {
            InitializeComponent();

            DataContextChanged += (a, b) =>
            {
                if (b.OldValue is MultiScreensViewModel oldvm)
                {
                    oldvm.Config.AllScreens.CollectionChanged -= AllScreens_CollectionChanged;
                }

                if (b.NewValue is MultiScreensViewModel newvm)
                {
                    AllScreens_CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,newvm.Config.AllScreens));
                    newvm.Config.AllScreens.CollectionChanged += AllScreens_CollectionChanged;
                }
            };
        }

        private void AllScreens_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var s in e.OldItems.OfType<Screen>())
                {
                    foreach (var element in Canvas.Children.OfType<FrameworkElement>().ToList())
                    {
                        if(element.DataContext is ScreenFrameView view && ReferenceEquals(view.ViewModel.Model,s))
                            Canvas.Children.Remove(element);
                    }
                }
                
            }

            if (e.NewItems != null)
            {
                foreach (var s in e.NewItems.OfType<Screen>())
                {
                    var view = ViewModel.Context.GetView<ViewModeDefault>(s, typeof(IViewClassDefault));
                    Canvas.Children.Add(view);
                }
            }
        }

        //protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        //{
        //    base.OnRenderSizeChanged(sizeInfo);

        //    foreach (var frameView in Canvas.Children.OfType<ScreenFrameView>())
        //    {
        //        frameView.SetPosition();
        //    }
        //}

        internal MultiScreensViewModel ViewModel => DataContext as MultiScreensViewModel;

        private ScreenConfig Config => ViewModel.Config;

        public double GetRatio()
        {
            if (Config == null) return 1;

            Rect all = Config.PhysicalOutsideBounds;

            if (all.Width * all.Height > 0)
            {
                return Math.Min(
                    BackgoundGrid.ActualWidth / all.Width,
                    BackgoundGrid.ActualHeight / all.Height
                );
            }
            return 1;
        }

        public double PhysicalToUiX(double x)
            => (x - Config.PhysicalOutsideBounds.Left - Config.PhysicalOutsideBounds.Width / 2) * GetRatio();


        public double PhysicalToUiY(double y)
            => (y - Config.PhysicalOutsideBounds.Top - Config.PhysicalOutsideBounds.Height / 2) * GetRatio();

    }
}
