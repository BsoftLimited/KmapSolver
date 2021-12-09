using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KmapSolver;
using KmapSolver.Components;
using KmapSolver.FlipFlops;
using KmapSolver.Solver;
using KmapSolver.Utils;

namespace KmapSolver{
	public class Table : VBox{
		public MyGrid grid;
		public FlopContainer flopContainer;
		
		public Table(){
			this.grid=new MyGrid();
			grid.ShowGridLines = true;
			this.flopContainer = new FlopContainer(this);
			this.Children.Add(grid);
		}
		
		public void InitailizeTable(VBox form){
			flopContainer.clear();
			foreach(var variable in form.Children){
				if(variable is Variables){
					switch((variable as Variables).getChoice()){
						case "output":
							flopContainer.add(new Output((variable as Variables).getName(), "0", "0"));
							break;
						case "input":
							flopContainer.add(new Input((variable as Variables).getName(),"0"));
							break;
						case "JK output":
							flopContainer.add(new JK((variable as Variables).getName(), "0", "0"));
							break;
						case "SR output":
							flopContainer.add(new SR((variable as Variables).getName(), "0", "0"));
							break;
					}
				}
			}
		}
		
		public void process(TableItemButton tableItemButton){
			foreach(FlipFlop f in flopContainer.SRAndJK){
				if(f.nextIndex.Equals(Grid.GetColumn(tableItemButton))){
					var now=this.grid.getChildren(f.nowIndex,Grid.GetRow(tableItemButton)) as TableItem;
					var next=this.grid.getChildren(f.nextIndex,Grid.GetRow(tableItemButton)) as TableItemButton;
					try {
						f.setNow(now.Text);
						f.setNext(next.Content.ToString());
						(this.grid.getChildren(f.aIndex,Grid.GetRow(tableItemButton)) as TableItem).Text=f.getA();
						(this.grid.getChildren(f.bIndex,Grid.GetRow(tableItemButton)) as TableItem).Text=f.getB();
						break;
					}catch (Exception e){
						Console.WriteLine(e.StackTrace);
					}
					return;
				}
			}
		}
	}
	
	public class Map :DockPanel{
		public MyGrid grid=new MyGrid();
		public String[,] values;
		List<String> output;
		public String sop;
		public MySolver solver;
		MyLabel buttomLabel;
		public Map(Table table,List<String> output,int index) {
			this.output=output;
			buttomLabel = new MyLabel(sop);
			buttomLabel.setDock(Dock.Bottom);
			this.Children.Add(buttomLabel);
			grid.ShowGridLines=true;
			grid.addRow(30);
			grid.addRow(30);
			grid.addColumn(30);
			grid.addColumn(30);
			if(table.flopContainer.SRAndJK.Count + table.flopContainer.inputs.Count==3){
				grid.addColumn(30);
				grid.addColumn(30);
			}else if(table.flopContainer.SRAndJK.Count + table.flopContainer.inputs.Count>3){
				grid.addRow(30);
				grid.addRow(30);
				grid.addColumn(30);
				grid.addColumn(30);
			}
			
			values= new String[grid.RowDefinitions.Count,grid.ColumnDefinitions.Count];
			String variable="";
			foreach(String input in table.flopContainer.getInputNames()){
				variable+=input;
			}
			switch(variable.Length){
				case 2:
					this.Children.Add(new MyLabel(variable.Substring(variable.Length-1,1)).setDock(Dock.Top));
					this.Children.Add(new MyLabel(variable.Substring(0,1)).setDock(Dock.Left));
					break;
				case 3:
					this.Children.Add(new MyLabel(variable.Substring(variable.Length-2,2)).setDock(Dock.Top));
					this.Children.Add(new MyLabel(variable.Substring(0,1)).setDock(Dock.Left));
					break;
				default:
					var sub=variable.Substring(0,variable.Length-4);
					this.Children.Add(new MyLabel(variable.Substring(variable.Length-2,2)).setDock(Dock.Top));
					this.Children.Add(new MyLabel(this.underline(sub,index)+variable.Substring(variable.Length-4,2)).setDock(Dock.Left));
					break;
			}
			this.Margin = new Thickness(15);
			fill();
			this.Children.Add(this.grid);
		}
		

		void fill(){
			for(int i=0; i<grid.RowDefinitions.Count;i++){
				for(int j=0;j<grid.ColumnDefinitions.Count;j++){
					var init=output[((i*grid.ColumnDefinitions.Count)+j)];
					grid.add(new TableItem(init),count(j),count(i));
					values[count(i),count(j)]=init;
				}
			}
			solver=new MySolver(this);
			solver.Update += update;
			solver.RunWorkerAsync();
		}
		
		void update(String data){
			sop = data;
			buttomLabel.Content = sop;
		}

		String underline(String sub,int index){
			var bits=Util.toBin(sub.Length,index);
			String results="";
			for(int i=0;i< sub.Length;i++){
				if(bits[i].Equals('1')){
					results+=sub[i].ToString();
				}else{
					results+=sub[i]+"\u0305";
				}
			}
			return results;
		}
		
		public UIElement getDockedChildren(Dock dock){
			foreach(UIElement child in Children){
				if(DockPanel.GetDock(child).Equals(dock)){
					return child;
				}
			}
			return null;
		}

		int count(int index){
			switch(index){
				case 2:
					return 3;
				case 3:
					return 2;
				default:
					return index;
			}
		}
	}
	
	public class KMap :VBox{
		private List<String> outputs;
		private KmapContainer container;
		public KMap(Table table,int columnIndex){
			outputs= new List<String>();
			var label = new TextBlock();
			label.Text =table.grid.getChildren(columnIndex, 0) is TableItem ? (table.grid.getChildren(columnIndex, 0) as TableItem).Text :(table.grid.getChildren(columnIndex, 0) as TableItemButton).Content.ToString();
			container=new KmapContainer();
			container.setTitle(label);
			this.Children.Add(container);
			this.Margin = new Thickness(0, 0, 0, 5);
			for(int i=1;i< table.grid.RowDefinitions.Count;i++){
				var init=table.grid.getChildren(columnIndex, i);
				if(init is TableItem){
					outputs.Add((init as TableItem).Text);
				}else{
					outputs.Add((init as TableItemButton).Content.ToString());
				}
			}
			for(int i=0; i<limit(table.flopContainer.SRAndJK.Count + table.flopContainer.inputs.Count);i++){
				var sub=new List<String>();
				if(outputs.Count>16) {
					for(int j= i*16;j<=i*16+15;j++){
						sub.Add(outputs[j]);
					}
				}else{
					foreach(String output in outputs){
						sub.Add(output);
					}
				}
				container.Add(new Map(table,sub,i));

			}
		}
		int limit(int count){
			return count < 4 ? 1 : (int)Math.Pow(2, count - 4);
		}
	}
	
	sealed class KmapContainer: Border{
		readonly WrapPanel panel;
		readonly VBox root;
		int count;
		public KmapContainer(){
			this.Background = Brushes.Transparent;
			panel = new WrapPanel();
			root = new VBox();
			var border = new Border();
			border.CornerRadius = new CornerRadius(4);
			border.Background = Brushes.WhiteSmoke;
			border.Margin = new Thickness(2);
			border.Child = root;
			root.Children.Add(panel);
			this.CornerRadius = new CornerRadius(4);
			this.Background = Brushes.LightSlateGray;
			this.Child = border;
			count = 0;
		}
		
		public void Add(Map child){
			child.solver.baseUpdate += setBase;
			panel.Children.Add(child);
		}
		
		public void setTitle(UIElement child){
			root.Children.Insert(0, child);
		}
		
		public void setBase(){
			count += 1;
			if(count.Equals(panel.Children.Count)){
				var init = new Label();
				init.HorizontalAlignment = HorizontalAlignment.Center;
				String sop=(root.Children[0] as TextBlock).Text+"=";
				foreach(Map map in panel.Children){
					if (!map.sop.Equals("0")) {
						sop +=(panel.Children.IndexOf(map).Equals(0)) ? map.sop:"+" + map.sop;
					}
				}
				if (sop[sop.Length-1].Equals('=')) {
					sop += "0";
				}else if (sop[0].Equals('+')) {
					sop=sop.Substring(1,sop.Length-1);
				}
				init.Content=sop;
				var simplifier = new Simplifier(init);
				simplifier.RunWorkerAsync();
				root.Children.Add(init);
			}
		}
	}
}
