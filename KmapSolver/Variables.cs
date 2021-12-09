/*
 * Created by SharpDevelop.
 * User: okelekele kiensue
 * Date: 01/19/2019
 * Time: 17:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using KmapSolver.Components;

namespace KmapSolver{
	public delegate void checkNames(object sender, EventArgs args);
	public class Variables: HBox{
		private TextInput name;
		private MyCombo choice;
		private MyButton removeButton;
		private VBox form;
		public checkNames CheckNames;
		public Variables(VBox form,Checker checker){
			this.form = form;
			this.Margin =new Thickness(5, 0, 5, 5);
			name = new TextInput();
			name.setHeight(20);
			name.setWidth(20);
			name.Margin = new Thickness(0, 0, 5, 0);
			name.Tag = "Variavble Name";
			name.textChanged += checker.checkNames;
			
			choice = new MyCombo("input","output","JK output","SR output");
			choice.Height = 20;
			choice.Width = 80;
			choice.Margin = new Thickness(0,0,5,0);
			
			removeButton = new MyButton("remove");
			removeButton.Margin = new Thickness(0, 0, 0, 0);
			removeButton.setBorderRadius(0, 4, 4, 0);
			removeButton.Click += removeFun;
			
			this.Children.Add(name);
			this.Children.Add(choice);
			this.Children.Add(removeButton);
		}
		
		void removeFun(object sender,RoutedEventArgs e){
			form.Children.RemoveAt(form.Children.IndexOf(this));
			CheckNames(this, e);
		}
		
		public String getName(){
			return name.getText();
		}
		
		public String getChoice(){
			return choice.getSelected();
		}
	}
	
	public class TableItem :TextBlock{
		public TableItem(String name){
			this.Text = name;
			this.HorizontalAlignment = HorizontalAlignment.Center;
			this.VerticalAlignment = VerticalAlignment.Center;
		}
	}
	
	public class TableItemButton:Button{
		readonly Table table;
		public TableItemButton(Table table,String name){
			Content = name;
			this.Click += change;
			this.table = table;
		}
		
		void change(object sender, RoutedEventArgs args){
			switch(Content.ToString()){
				case "0":
					this.Content = "1";
					table.process(this);
					break;
				case "1":
					this.Content = "0";
					table.process(this);
					break;
			}
		}
	}
}
