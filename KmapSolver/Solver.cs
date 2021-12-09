using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using KmapSolver;
using KmapSolver.Components;

namespace KmapSolver.Solver{
	public delegate void UpdadeListrner(String sop);
	public delegate void BaseUpdater();
	public class MySolver: BackgroundWorker{
		Boolean[,] check;
		String[,] slots;
		int rowCount;
		int columnCount;
		String output,left,top;
		readonly Map map;
		public UpdadeListrner Update;
		public BaseUpdater baseUpdate;
		
		class Cell {
			public int rIndex, cIndex, rLength,cLength;
			public bool minus;
			public Cell(int rIndex,int rLength,int cIndex,int cLength,bool minus){
				this.rIndex=rIndex;
				this.cIndex=cIndex;
				this.cLength = cLength;
				this.rLength = rLength;
				this.minus = minus;
			}
		}

		public MySolver(Map map){
			this.map=map;
			this.WorkerReportsProgress = true;
			rowCount = this.map.grid.RowDefinitions.Count;
			columnCount = this.map.grid.ColumnDefinitions.Count;
			check = new Boolean[rowCount,columnCount];
			slots = map.values;
			this.DoWork += running;
			this.ProgressChanged+=change;
			left=(map.getDockedChildren(Dock.Left) as MyLabel).Content.ToString();
			top=(map.getDockedChildren(Dock.Top) as MyLabel).Content.ToString();
		}
		
		void initailize(){
			output = "";
			for (int i=0; i<rowCount;i++) {
				for (int j= 0;j<columnCount;j++){
					check[i,j]=false;
				}
			}
		}
		
		void change(object sender, ProgressChangedEventArgs args){
			Update(output);
			baseUpdate();
		}
		
		void running(object sender,DoWorkEventArgs args){
			this.initailize();
			this.compute();
			this.ReportProgress(100);
		}

		bool filled(){
			for (int i=0; i<rowCount;i++) {
				for (int j=0;j<columnCount;j++) {
					if (!check[i,j]) {
						return false;
					}
				}
			}
			return true;
		}

		void fill(int r, int rLength,int c,int cLength,bool minus){
			for (int i=0;i<rLength;i++) {
				for (int j=0;j<cLength;j++) {
					int y;
					int x;
					if(minus){
						y =(this.rowCount+ (r - i))% rowCount;
						x =(this.columnCount+ (c - j))% columnCount;
					} else {
						y = (r + i) % this.rowCount;
						x = (c + j) % this.columnCount;
					}
					check[y,x]=true;
				}
			}
		}
		
		bool inner(int r,int c,int rLength,int cLength,bool minus){
			for(int i=0;i<rLength; i++){
				for(int j=0;j<cLength;j++){
					int y;
					int x;
					if(minus){
						y =(this.rowCount+ (r - i))% rowCount;
						x =(this.columnCount+ (c - j))% columnCount;
					}else{
						y = (r + i) % this.rowCount;
						x = (c + j) % this.columnCount;
					}
					if (slots[y,x].Equals("0")) {
						return false;
					}
				}
			}
			return true;
		}

		Cell run(int rLength,int cLength){
			for(int r=0; r<rowCount;r++){
				for(int c=0;c<columnCount; c++){
					if (!check[r, c]) {
						if (slots[r, c].Equals("1") && inner(r, c, rLength, cLength, false)) {
							this.fill(r, rLength, c, cLength, false);
							return new Cell(r, rLength, c, cLength, false);
						} else if (slots[r, c].Equals("1") && inner(r, c, rLength, cLength, true)) {
							this.fill(r, rLength, c, cLength, true);
							return new Cell(r, rLength, c, cLength, true);
						}
					}
				}
			}
			return null;
		}

		

		String readCSingle(Cell cell){
			if(top.Length.Equals(1)){
				return "";
			} else {
				switch (cell.cIndex) {
					case 0:
						return cell.minus ? top.Substring(top.Length - 1,1) + "\u0305" : top.Substring(top.Length - 2,1) + "\u0305";
					case 1:
						return cell.minus ? top.Substring(top.Length - 2,1) + "\u0305" : top.Substring(top.Length - 1,1);
					case 2:
						return cell.minus ? top.Substring(top.Length - 1,1) : top.Substring(top.Length - 2,1);
					case 3:
						return cell.minus ? top.Substring(top.Length - 2,1) : top.Substring(top.Length - 1,1) + "\u0305";
				}
			}
			return null;
		}

		String readCDouble(Cell cell){
			if (top.Length > 1) {
				switch (cell.cIndex) {
					case 0:
						return top.Substring(top.Length - 2,1) + "\u0305" + top.Substring(top.Length - 1,1) + "\u0305";
					case 1:
						return top.Substring(top.Length - 2,1) + "\u0305" + top.Substring(top.Length - 1,1);
					case 2:
						return top.Substring(top.Length - 2,2);
					case 3:
						return top.Substring(top.Length - 2,2) + "\u0305";
				}
			}else{
				switch (cell.cIndex) {
					case 0:
						return top.Substring(top.Length - 1,1) + "\u0305";
					case 1:
						return top.Substring(top.Length - 1,1);
				}
			}
			return null;
		}

		String readRSingle(Cell cell){
			if(left.Length.Equals(1)){
				return "";
			} else {
				switch (cell.rIndex) {
					case 0:
						return cell.minus ? left.Substring(left.Length - 1,1) + "\u0305" : left.Substring(left.Length - 2,1) + "\u0305";
					case 1:
						return cell.minus ? left.Substring(left.Length - 2,1) + "\u0305" : left.Substring(left.Length - 1,1);
					case 2:
						return cell.minus ? left.Substring(left.Length - 1,1) : left.Substring(left.Length - 2,1);
					case 3:
						return cell.minus ? left.Substring(left.Length - 2,1) : left.Substring(left.Length - 1,1) + "\u0305";
				}
			}
			return null;
		}

		String readRDouble(Cell cell){
			if(left.Length.Equals(1)){
				switch (cell.rIndex) {
					case 0:
						return left.Substring(left.Length - 1,1) + "\u0305";
					case 1:
						return left.Substring(left.Length - 1,1);
				}
			} else {
				switch (cell.rIndex) {
					case 0:
						return left.Substring(left.Length - 2,1) + "\u0305" + left.Substring(left.Length - 1,1) + "\u0305";
					case 1:
						return left.Substring(left.Length - 2,1) + "\u0305" + left.Substring(left.Length - 1,1);
					case 2:
						return left.Substring(left.Length - 2, 2);
					case 3:
						return left.Substring(left.Length - 2,2) + "\u0305";
				}
			}
			return null;
		}

		void process(int rLength,int cLength){
			var init=run(rLength,cLength);
			while (init !=null){
				String a="";
				switch(rLength){
					case 2:
						a+=readRSingle(init);
						break;
					case 1:
						a+=readRDouble(init);
						break;
				}

				switch(cLength){
					case 2:
						a+=readCSingle(init);
						break;
					case 1:
						a+=readCDouble(init);
						break;
				}
				if(left.Length>=3){
					a=left.Substring(0,left.Length-2)+a;
				}
				if(!a.Equals("")){
					output=output+"+"+a;
				}else{
					output=output+"+1";
				}
				init=run(rLength,cLength);
			}
		}

		void check16(){
			if(columnCount==4 && rowCount==4) {
				process(4, 4);
			}
		}

		void check8(){
			if(columnCount==4) {
				process(2, 4);
			}
			if(rowCount==4) {
				process(4, 2);
			}
		}

		void check4(){
			if(columnCount==4) {
				process(1, 4);
			}
			if(rowCount==4) {
				process(4, 1);
			}
			process(2,2);
		}

		void check2(){
			process(1,2);
			process(2,1);
		}

		void check1(){
			process(1,1);
		}

		public void compute(){
			check16();
			check8();
			check4();
			check2();
			check1();
			if(output.Length>=1 && output[0].Equals('+')){
				output = output.Substring(1, output.Length - 1);
			}
			if(output.Equals("")){
				output = "0";
			}
		}
	}
	
	public class Simplifier:BackgroundWorker{
		Label label;
		String output;
		String input;
		public Simplifier(Label label){
			this.label = label;
			this.input = label.Content.ToString();
			this.WorkerReportsProgress = true;
			this.DoWork += task;
			this.ProgressChanged += Complete;
		}
		
		void task(object sender,DoWorkEventArgs args){
			output = input.Substring(0, 2);
			input = input.Substring(2, input.Length - 2);
			if (input.Contains("+")) {
				process();
			}
			this.ReportProgress(0);
		}
		
		void Complete(object sender,ProgressChangedEventArgs args){
			output += input;
			label.Content = output;
			Debug.WriteLine("Simplifier complete");
		}
		
		void process(){
			int maxLength=this.getMaxLength();
			Debug.WriteLine(maxLength);
		}
		
		int getMaxLength(){
			int index = 0,n=0;
			for(int i=0;index<input.Length;i++){
				if(input[i].Equals("+")){
					if(n<index){
						n = index;
					}
					index = 0;
				}else if(input[i].Equals("\u0305")){
					index += 1;
				}
			}
			return n;
		}
	}
}
