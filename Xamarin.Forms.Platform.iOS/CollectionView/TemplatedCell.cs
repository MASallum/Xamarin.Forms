﻿using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	// TODO hartez 2018/09/17 14:11:02 Should this be named "TemplateCell" instead of "TemplatedCell"?	
	public abstract class TemplatedCell : ItemsViewCell
	{
		protected nfloat ConstrainedDimension;

		[Export("initWithFrame:")]
		protected TemplatedCell(CGRect frame) : base(frame)
		{
		}

		public IVisualElementRenderer VisualElementRenderer { get; private set; }

		public override void ConstrainTo(nfloat constant)
		{
			ConstrainedDimension = constant;
		}

		public override UICollectionViewLayoutAttributes PreferredLayoutAttributesFittingAttributes(
			UICollectionViewLayoutAttributes layoutAttributes)
		{
			var nativeView = VisualElementRenderer.NativeView;

			var size = Measure();

			nativeView.Frame = new CGRect(CGPoint.Empty, size);
			VisualElementRenderer.Element.Layout(nativeView.Frame.ToRectangle());

			layoutAttributes.Frame = nativeView.Frame;

			return layoutAttributes;
		}

		public void SetRenderer(IVisualElementRenderer renderer)
		{
			ClearSubviews();

			VisualElementRenderer = renderer;
			var nativeView = VisualElementRenderer.NativeView;

			InitializeContentConstraints(nativeView);
		}

		protected void Layout(CGSize constraints)
		{
			var nativeView = VisualElementRenderer.NativeView;

			var width = constraints.Width;
			var height = constraints.Height;

			VisualElementRenderer.Element.Measure(width, height, MeasureFlags.IncludeMargins);

			nativeView.Frame = new CGRect(0, 0, width, height);

			VisualElementRenderer.Element.Layout(nativeView.Frame.ToRectangle());
		}

		void ClearSubviews()
		{
			for (int n = ContentView.Subviews.Length - 1; n >= 0; n--)
			{
				// TODO hartez 2018/09/07 16:14:43 Does this also need to clear the constraints?	
				ContentView.Subviews[n].RemoveFromSuperview();
			}
		}

		// TODO hartez Split this into another level, the header/foot views shouldn't be selectable
		public override bool Selected
		{
			get => base.Selected;
			set
			{
				base.Selected = value;

				var element = VisualElementRenderer?.Element;

				if (element != null)
				{
					VisualStateManager.GoToState(element, value 
						? VisualStateManager.CommonStates.Selected 
						: VisualStateManager.CommonStates.Normal);
				}
			}
		}
	}

	// TODO hartez think about just changing these to 'cell' instead of 'view'
	public class HorizontalTemplatedHeaderView : TemplatedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.HorizontalHeaderView");

		[Export("initWithFrame:")]
		public HorizontalTemplatedHeaderView(CGRect frame) : base(frame)
		{
		}

		public override CGSize Measure()
		{
			var measure = VisualElementRenderer.Element.Measure(double.PositiveInfinity, 
				ConstrainedDimension, MeasureFlags.IncludeMargins);

			var width = VisualElementRenderer.Element.Width > 0 
				? VisualElementRenderer.Element.Width : measure.Request.Width;

			return new CGSize(width, ConstrainedDimension);
		}

		public override void ConstrainTo(CGSize constraint)
		{
			ConstrainedDimension = constraint.Height;
			Layout(constraint);
		}
	}

	public class VerticalTemplatedHeaderView : TemplatedCell
	{
		public static NSString ReuseId = new NSString("Xamarin.Forms.Platform.iOS.VerticalTemplatedHeaderView");

		[Export("initWithFrame:")]
		public VerticalTemplatedHeaderView(CGRect frame) : base(frame)
		{
		}

		public override CGSize Measure()
		{
			var measure = VisualElementRenderer.Element.Measure(ConstrainedDimension, 
				double.PositiveInfinity, MeasureFlags.IncludeMargins);

			var height = VisualElementRenderer.Element.Height > 0 
				? VisualElementRenderer.Element.Height : measure.Request.Height;

			return new CGSize(ConstrainedDimension, height);
		}

		public override void ConstrainTo(CGSize constraint)
		{
			ConstrainedDimension = constraint.Width;
			Layout(constraint);
		}
	}
}