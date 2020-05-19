using System;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace GongSolutions.Wpf.DragDrop
{
  internal class DragAdorner : Adorner
  {
    static DragAdorner()
    {
      m_AdornerLayer = new Popup()
      {
        StaysOpen = true,
        IsOpen = false,
        Placement = PlacementMode.Custom,
        PlacementTarget = Application.Current.MainWindow,
        AllowsTransparency = true,
        CustomPopupPlacementCallback = CustomPopupPlacementCallback,
        IsHitTestVisible = false
      };
    }

    private static CustomPopupPlacement[] CustomPopupPlacementCallback(Size popupsize, Size targetsize, Point offset)
    {
      return new[] { new CustomPopupPlacement(new Point(20, -popupsize.Height - 10), PopupPrimaryAxis.None) };
    }

    public DragAdorner(UIElement adornedElement, UIElement adornment, DragDropEffects effects = DragDropEffects.None)
      : base(adornedElement)
    {
      m_AdornerLayer.Child = this;
      this.m_Adornment = adornment;
      this.IsHitTestVisible = false;
      this.Effects = effects;
      m_AdornerLayer.IsOpen = true;

      System.Windows.DragDrop.AddPreviewDragOverHandler(Window.GetWindow(AdornedElement), Handler);
      System.Windows.DragDrop.AddPreviewDragOverHandler(m_AdornerLayer.Child, Handler);
    }

    private void Handler(object sender, DragEventArgs e)
    {
      var position = e.GetPosition(Window.GetWindow(AdornedElement));
      m_AdornerLayer.PlacementRectangle = new Rect(position.X, position.Y, ActualWidth, ActualHeight);
    }

    public DragDropEffects Effects { get; private set; }

    public Point MousePosition
    {
      get { return this.m_MousePosition; }
      set
      {
        if (this.m_MousePosition != value)
        {
          this.m_MousePosition = value;
          UpdatePosition();
        }
      }
    }

    public void Detatch()
    {
      m_AdornerLayer.IsOpen = false;
      System.Windows.DragDrop.RemovePreviewDragOverHandler(Window.GetWindow(AdornedElement), Handler);
      System.Windows.DragDrop.RemovePreviewDragOverHandler(m_AdornerLayer.Child, Handler);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      this.m_Adornment.Arrange(new Rect(finalSize));
      return finalSize;
    }

    protected override Visual GetVisualChild(int index)
    {
      return this.m_Adornment;
    }

    protected override Size MeasureOverride(Size constraint)
    {
      this.m_Adornment.Measure(constraint);
      return this.m_Adornment.DesiredSize;
    }

    protected override int VisualChildrenCount
    {
      get { return 1; }
    }

    private static readonly Popup m_AdornerLayer;
    private readonly UIElement m_Adornment;
    private Point m_MousePosition;

    public void UpdatePosition()
    {
    }
  }
}