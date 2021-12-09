/*
 * Created by SharpDevelop.
 * User: okelekele kiensue
 * Date: 1/24/2019
 * Time: 6:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KmapSolver.Components{
	public delegate void click(object sender,RoutedEventArgs args);
	public delegate void TextChanged(object sender,EventArgs args);
	
	public interface Component{
		void addAll(params UIElement[] children);
		UIElement setDock(Dock dock);
	}
	
	public class VBox: StackPanel,Component{
		public VBox(){
			this.Orientation=Orientation.Vertical;
		}

		public void addAll(params UIElement[] children){
			foreach(UIElement child in children){
				this.Children.Add(child);
			}
		}

		public UIElement setDock(Dock dock){
			DockPanel.SetDock(this, dock);
			return this;
		}
	}
	
	public class HBox: VBox{
		public HBox(){
			Orientation = Orientation.Horizontal;
		}
	}
	
	public class MyGrid:Grid,Component{
		public MyGrid(){
			this.Background = Brushes.RoyalBlue;
		}
		public void add(UIElement element,int columnIndex,int rowIndex){
			Grid.SetRow(element,rowIndex);
			Grid.SetColumn(element,columnIndex);
			this.Children.Add(element);
		}

		public void addAll(params UIElement[] children){
			foreach(UIElement child in children){
				Children.Add(child);
			}
		}

		public UIElement setDock(Dock dock){
			DockPanel.SetDock(this, dock);
			return this;
		}
		
		public void addRow(int height){
			var init=new RowDefinition();
			init.Height=new GridLength(height,GridUnitType.Pixel);
			this.RowDefinitions.Add(init);
		}
		
		public void addColumn(int width){
			var init=new ColumnDefinition();
			init.Width = new GridLength(width,GridUnitType.Pixel);
			this.ColumnDefinitions.Add(init);
		}
		
		public void sortChildren(){
			this.Height = RowDefinitions[0].Height.Value * RowDefinitions.Count;
			this.Width = ColumnDefinitions[0].Width.Value * ColumnDefinitions.Count;
		}
		
		public UIElement getChildren(int index,int rowIndex){
			foreach(UIElement child in this.Children){
				try {
					if (Grid.GetColumn(child).Equals(index) && Grid.GetRow(child).Equals(rowIndex)) {
						return child;
					}
				}catch (Exception e){
					Console.WriteLine(e.StackTrace);
				}
			}
			return null;
		}
	}
	
	public class MyLabel:Label,Component{
		public MyLabel(String statement){
			this.Content=statement;
			this.VerticalAlignment = VerticalAlignment.Center;
			this.HorizontalAlignment = HorizontalAlignment.Center;
		}
		
		public void addAll(params UIElement[] children){
		}

		public UIElement setDock(Dock dock){
			DockPanel.SetDock(this, dock);
			return this;
		}
	}
	
	public class MyCombo: ComboBox{
		String[] i;
		public MyCombo(params String[] items){
			foreach(var item in items){
				var init=new ComboBoxItem();
				init.Content=item;
				this.Items.Add(init);
			}
			this.SelectedIndex = 0;
			i = items;
		}
		
		public String getSelected(){
			return i[this.SelectedIndex];
		}
	}
	
	public sealed class MyButton:Border{
		Button btn;
		Border border;
		public event click Click;
		public MyButton(String title){
			btn = new Button();
			btn.Content = title;
			btn.Background = Brushes.Transparent;
			btn.BorderThickness = new Thickness(0);
			btn.Click += this.pressed;
			border = new Border();
			border.CornerRadius = new CornerRadius(2.5);
			border.Background = Brushes.WhiteSmoke;
			border.Margin = new Thickness(1);
			border.Child = btn;
			this.CornerRadius = new CornerRadius(2.5);
			this.Background = Brushes.LightSlateGray;
			this.Child = border;		
		}
		
		public void setBorderRadius(double radius){
			this.CornerRadius = new CornerRadius(radius);
			border.CornerRadius = new CornerRadius(radius);
		}
		
		public void setBorderRadius(double topleft,double topright,double buttomright,double buttomleft){
			this.CornerRadius = new CornerRadius(topleft,topright,buttomright,buttomleft);
			border.CornerRadius = new CornerRadius(topleft,topright,buttomright,buttomleft);
		}
		
		
		public void setBorderThickness(double thickness){
			border.Margin = new Thickness(thickness);
		}
		
		void pressed(object sender, RoutedEventArgs args){
			Click(sender,args);
		}
	}
	
	sealed class TextInput:Border{
		readonly TextBox text;
		public event TextChanged textChanged;
		Border border;
		public TextInput(){
			text= new TextBox();
			text.Background = Brushes.Transparent;
			text.BorderThickness = new Thickness(0);
			text.KeyUp += this.pressed;
			border = new Border();
			border.CornerRadius = new CornerRadius(5,0,0,5);
			border.Background = Brushes.WhiteSmoke;
			border.Margin = new Thickness(1);
			border.Child = text;
			this.CornerRadius = new CornerRadius(5,0,0,5);
			this.Background = Brushes.LightSlateGray;
			this.Child = border;		
		}
		
		void pressed(object sender,EventArgs args){
			if(text.Text.Length>1){
				text.Text = text.Text.Substring(0, 1);
			}
			textChanged(sender, args);
		}
		
		public String getText(){
			return text.Text;
		}
		
		public void setHeight(int height){
			text.Height = height-2;
		}
		
		public void setWidth(int width){
			text.Width = width-2;
		}
	}
}
