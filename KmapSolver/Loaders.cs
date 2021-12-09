using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using KmapSolver.Components;
using KmapSolver.FlipFlops;

namespace KmapSolver.Loaders{
	public abstract class Loader: BackgroundWorker{
		protected readonly LoaderListener parent;
		public bool isRunning,updating;
		public int progressValue;
		
		protected Loader(LoaderListener parent){
			this.WorkerReportsProgress = true;
			this.DoWork += this.running;
			this.ProgressChanged += this.change;
			this.RunWorkerCompleted += this.complete;
			this.parent = parent;
			progressValue = 0;
			isRunning=false;
		}
		
		public void start(){
			this.RunWorkerAsync();
		}
		
		protected void makeUpdate(int percent){
			this.ReportProgress(percent);
		}
		
		void complete(object sender,RunWorkerCompletedEventArgs args){
			parent.onFinished(this);
			isRunning = false;
		}
		
		protected abstract void change(object sender, ProgressChangedEventArgs args);
		
		void running(object sender,DoWorkEventArgs args){
			parent.onStart(this);
			isRunning=true;
			Task();
		}
		
		abstract protected void  Task();
		
		public interface LoaderListener{
			void onStart(Loader loader);
			void onUpdate(Loader loader);
			void onFinished(Loader loader);
		}
	}
	
	public class KMapLoader: Loader{
		public VBox kMaps;
		public Table table;
		public KMapLoader(LoaderListener parent):base(parent){
			kMaps = new VBox();
		}

		protected override void Task(){
			this.makeUpdate(5);
		}

		protected override void change(object sender, ProgressChangedEventArgs args){
			progressValue = 0;
			parent.onUpdate(this);
			int total = table.flopContainer.SRAndJK.Count + table.flopContainer.outputs.Count;
			int index = 0;
			foreach(FlipFlop flop in table.flopContainer.SRAndJK){
				progressValue = (index / total) * 100;
				parent.onUpdate(this);
				kMaps.Children.Add(new KMap(table,flop.aIndex));
				kMaps.Children.Add(new KMap(table, flop.bIndex));
				index += 1;
			}
			
			foreach(FlipFlop flop in table.flopContainer.outputs){
				progressValue = (index / total) * 100;
				parent.onUpdate(this);
				kMaps.Children.Add(new KMap(table,flop.nowIndex));
				index += 1;
			}
			progressValue = 100;
			parent.onUpdate(this);
		}
		
		public void refresh(Table table){
			this.table=table;
			kMaps.Children.Clear();
			Debug.Print("kmap loading");
			this.start();
		}
	}
	
	public class TableLoader: Loader{
		public Table table;
		public VBox form;
		public TableLoader(LoaderListener parent):base(parent){
			
		}

		protected override void Task(){
			this.makeUpdate(0);
			Debug.WriteLine("finished");
		}
		
		public void set(VBox form){
			this.form = form;
			progressValue = 0;
			//parent.onUpdate(this);
			Debug.WriteLine("setting up table");
			table = new Table();
			progressValue = 20;
			//parent.onUpdate(this);
			Debug.WriteLine("initailing table");
			table.InitailizeTable(form);
			progressValue = 30;
			///parent.onUpdate(this);
			Debug.WriteLine("arranging table");
			table.flopContainer.arrange(table);
			progressValue = 50;
			//parent.onUpdate(this);
			Debug.WriteLine("filling table");
			table.flopContainer.fill(table);
			progressValue = 80;
			//parent.onUpdate(this);
			table.grid.sortChildren();
			progressValue = 100;
			//parent.onUpdate(this);
			this.start();
		}

		protected override void change(object sender, ProgressChangedEventArgs args){
			
		}
	}
}
