/*
 * Created by SharpDevelop.
 * User: okelekele nobel bobby
 * Date: 1/18/2019
 * Time: 5:19 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using KmapSolver.Components;
using KmapSolver.FlipFlops;

namespace KmapSolver{
	public partial class Root : Window,Checker{
		public VBox form;
		Table table;
		VBox kMaps;
		About aboutWindow;
		Manual manual;
		public Root(){
			this.WindowStyle = WindowStyle.ThreeDBorderWindow;
			InitializeComponent();
			form=new VBox();
			form.Margin=new Thickness(0, 5, 0, 0);
			form.HorizontalAlignment = HorizontalAlignment.Center;
			form.Children.Add(new Variables(form,this));
			
			var addBtn = new MyButton("add");
			addBtn.Width = 0x32;
			addBtn.Click += addVariable;
			
			form.Children.Add(addBtn);
			content.Child=form;
		}
		
		void addVariable(object sender,RoutedEventArgs e){
			var init=new Variables(form,this);
			init.CheckNames += this.checkNames;
			form.Children.Insert(form.Children.Count-1,init);
			tabulateBtn.IsEnabled = false;
		}
		
		void showManual(object sender,RoutedEventArgs e){
			manual = new Manual();
			manual.WindowsListener += this.closePopWindows;
			manual.Show();
			manualMenu.IsEnabled = false;
		}
		
		void showHelp(object sender,RoutedEventArgs e){
			aboutWindow = new About();
			aboutWindow.WindowsListener += this.closePopWindows;
			aboutWindow.Show();
			aboutMenu.IsEnabled = false;
		}
		
		void closePopWindows(Window window){
			Debug.WriteLine("A window was closed"+ window.Title);
			switch(window.Title){
				case "Manual":
					manualMenu.IsEnabled = true;
					break;
				case "About":
					aboutMenu.IsEnabled = true;
					break;
			}
			
		}
		
		void Tabulate(object sender,RoutedEventArgs e){
			bar.Visibility = Visibility.Visible;
			container.Children.Clear();
			back.Visibility = Visibility.Hidden;
			computeBtn.Visibility = Visibility.Visible;
			bar.Value = 0;
			Debug.WriteLine("setting up table");
			table = new Table();
			bar.Value = 20;
			Debug.WriteLine("initailing table");
			table.InitailizeTable(form);
			bar.Value = 30;
			Debug.WriteLine("arranging table");
			table.flopContainer.arrange();
			bar.Value = 50;
			Debug.WriteLine("filling table");
			table.flopContainer.fill(table);
			bar.Value = 80;
			table.grid.sortChildren();
			bar.Value = 100;
			computeMenu.IsEnabled = true;
			container.Children.Add(table);
			bar.Visibility = Visibility.Hidden;
		}
		
		void Exit(object sender,RoutedEventArgs e){
			Application.Current.Shutdown();
		}
		
		void backFun(object sender,RoutedEventArgs e){
			container.Children.RemoveAt(container.Children.Count - 1);
			back.Visibility = Visibility.Hidden;
			computeMenu.IsEnabled = true;
			computeBtn.Visibility = Visibility.Visible;
		}
		
		void Compute(object sender,RoutedEventArgs e){
			bar.Visibility = Visibility.Visible;
			kMaps = new VBox();
			kMaps.Background = Brushes.WhiteSmoke;
			bar.Value = 0;
			int total = table.flopContainer.SRAndJK.Count + table.flopContainer.outputs.Count;
			int index = 0;
			foreach(FlipFlop flop in table.flopContainer.SRAndJK){
				bar.Value = (index / total) * 100;
				kMaps.Children.Add(new KMap(table,flop.aIndex));
				kMaps.Children.Add(new KMap(table, flop.bIndex));
				index += 1;
			}
			
			foreach(FlipFlop flop in table.flopContainer.outputs){
				bar.Value = (index / total) * 100;
				kMaps.Children.Add(new KMap(table,flop.nowIndex));
				index += 1;
			}
			bar.Value = 100;
			computeBtn.Visibility = Visibility.Hidden;
			computeMenu.IsEnabled = false;
			container.Children.Add(kMaps);
			back.Visibility = Visibility.Visible;
			bar.Visibility = Visibility.Hidden;
		}

		public void checkNames(object sender, EventArgs args){
			foreach(UIElement child in form.Children){
				try{
					if(child is Variables && (child as Variables).getName().Equals("")){
						tabulateBtn.IsEnabled = false;
						tabulateMenu.IsEnabled = false;
						return;
					}
				}catch(Exception e){
					Debug.WriteLine(e.StackTrace);
				}
			}
			tabulateBtn.IsEnabled = true;
			tabulateMenu.IsEnabled = true;
		}
		
		protected override void OnClosed(EventArgs e){
			base.OnClosed(e);
			Application.Current.Shutdown();
		}
		
		void Window_Loaded(object sender, RoutedEventArgs e){
			splash.Opacity = 0;
			DoubleAnimation animationIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(3));
			animationIn.EasingFunction = new QuadraticEase();
			
			Storyboard fadeIn = new Storyboard();
			fadeIn.Children.Add(animationIn);
			
			Storyboard.SetTarget(fadeIn,splash);
			Storyboard.SetTargetProperty(fadeIn, new PropertyPath(UIElement.OpacityProperty));
			
			DoubleAnimation animationOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(3));
			animationOut.EasingFunction = new QuadraticEase();
			animationOut.BeginTime = TimeSpan.FromSeconds(2);
			
			Storyboard fadeOut = new Storyboard();
			fadeOut.Children.Add(animationOut);
			
			Storyboard.SetTarget(fadeOut,splash);
			Storyboard.SetTargetProperty(fadeOut, new PropertyPath(UIElement.OpacityProperty));
			
			fadeIn.Completed+= delegate {
				fadeOut.Begin();
			};
			
			fadeOut.Completed+= delegate {
				splash.Visibility = Visibility.Collapsed;
				this.WindowStyle = WindowStyle.ThreeDBorderWindow;
				mainContent.Visibility = Visibility.Visible;
			};
			
			
			fadeIn.Begin();
		}
	}
	
	public interface Checker{
		void checkNames(object sender,EventArgs args);
	}
	
}