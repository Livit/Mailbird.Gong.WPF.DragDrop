using System;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace GongSolutions.Wpf.DragDrop
{
  public abstract class DropTargetAdorner : Adorner
  {
    static DropTargetAdorner()
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

    public DropTargetAdorner(UIElement adornedElement)
      : base(adornedElement)
    {
      m_AdornerLayer.Child = this;
      this.IsHitTestVisible = false;
      this.AllowDrop = false;
      this.SnapsToDevicePixels = true;
    }

    public void Detatch()
    {
      m_AdornerLayer.IsOpen = false;
    }

    public DropInfo DropInfo { get; set; }

    internal static DropTargetAdorner Create(Type type, UIElement adornedElement)
    {
      if (!typeof(DropTargetAdorner).IsAssignableFrom(type)) {
        throw new InvalidOperationException(
          "The requested adorner class does not derive from DropTargetAdorner.");
      }

      return (DropTargetAdorner)type.GetConstructor(new[] { typeof(UIElement) })
                                    .Invoke(new[] { adornedElement });
    }

    private static readonly Popup m_AdornerLayer;
  }
}