/*
 * Created by SharpDevelop.
 * User: okelekele kiensue
 * Date: 01/19/2019
 * Time: 19:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using KmapSolver;
using KmapSolver.Utils;

namespace KmapSolver.FlipFlops
{
	
	public abstract class FlipFlop{
		protected String a = "0";
		protected String b = "X";
		protected String now = "0";
		protected String next = "0";
		public int nowIndex = 0, nextIndex = 0, aIndex = 0, bIndex = 0;
		public String name;
		
		protected FlipFlop(String name){
			this.name = name;
		}
		
		public String getA(){
			return a;
		}
		
		public String getB(){
			return b;
		}
		
		public void setNow(String value){
			now = value;
			this.evaluate();
		}
		
		public String getNow(){
			return now;
		}
		
		public void setNext(String value)
		{
			next = value;
			this.evaluate();
		}
		
		public String getNext()
		{
			return next;
		}
		
		protected abstract void evaluate();
	}
	
	public class Output :FlipFlop{
		public Output(String name, String now, String next): base(name){
		}
		
		protected override void evaluate(){
			this.a = next;
			this.b = this.a;
		}
	}
	
	public class Input: FlipFlop{
		public Input(String name, String now)
			: base(name)
		{
			this.now = now;
		}

		protected override void evaluate()
		{
			
		}
	}
	
	public sealed class JK:FlipFlop{
		public JK(String name, String now, String next)
			: base(name)
		{
			this.now = now;
			this.next = next;
			this.name = name;
		}
		
		protected override void evaluate()
		{
			if (now.Equals("0") && next.Equals("0")) {
				this.a = "0";
				this.b = "X";
			} else if (now.Equals("0") && next.Equals("1")) {
				this.a = "1";
				this.b = "X";
			} else if (now.Equals("1") && next.Equals("0")) {
				this.a = "X";
				this.b = "1";
			} else if (now.Equals("1") && next.Equals("1")) {
				this.a = "X";
				this.b = "0";
			}
		}
	}
	
	sealed class SR :FlipFlop
	{
		public SR(String name, String now, String next)
			: base(name)
		{
			this.name = name;
			this.now = now;
			this.next = next;
			this.evaluate();
		}

		protected override void evaluate(){
			if (now.Equals("0") && next.Equals("0")) {
				this.a = "0";
				this.b = "X";
			} else if (now.Equals("0") && next.Equals("1")) {
				this.a = "1";
				this.b = "0";
			} else if (now.Equals("1") && next.Equals("0")) {
				this.a = "0";
				this.b = "1";
			} else if (now.Equals("1") && next.Equals("1")) {
				this.a = "X";
				this.b = "0";
			}
		}
	}
	
	public sealed class FlopContainer{
		public List<Output> outputs;
		public List<FlipFlop> SRAndJK;
		public List<Input> inputs;
		public int total;
		private Table table;
		
		public FlopContainer(Table table){
			outputs = new List<Output>();
			SRAndJK = new List<FlipFlop>();
			inputs = new List<Input>();
			this.table = table;
		}
		
		public void  add(FlipFlop flop){
			if (flop is Output) {
				outputs.Add(flop as Output);
			} else if (flop is Input) {
				inputs.Add(flop as Input);
			} else if (flop is JK || flop is SR) {
				SRAndJK.Add(flop);
			}
			total += 1;
		}
		
		public void clear(){
			outputs.Clear();
			SRAndJK.Clear();
			inputs.Clear();
		}
		
		public List<String> getInputNames(){
			var list = new List<String>();
			foreach (Input input in inputs) {
				list.Add(input.name);
			}
			
			foreach (FlipFlop flop in SRAndJK) {
				list.Add(flop.name);
			}
			return list;
		}
		
		public void arrange(){
			table.grid.addRow(30);
			int index = 0;
			foreach (Input flop in inputs) {
				table.grid.addColumn(30);
				flop.nowIndex = index;
				table.grid.add(new TableItem(flop.name), index, 0);
				index += 1;
			}
			
			foreach (FlipFlop flop in SRAndJK) {
				String aName;
				String bName;
				if (flop is JK) {
					aName = flop.name + "j";
					bName = flop.name + "k";
				} else {
					aName = flop.name + "s";
					bName = flop.name + "r";
				}
				flop.nowIndex = index;
				flop.nextIndex = index + SRAndJK.Count;
				flop.aIndex = flop.nowIndex + SRAndJK.IndexOf(flop) + (SRAndJK.Count * 2);
				flop.bIndex = flop.aIndex + 1;
				table.grid.addColumn(30);
				table.grid.add(new TableItem(flop.name), flop.nowIndex, 0);
				table.grid.addColumn(30);
				table.grid.add(new TableItem(flop.name), flop.nextIndex, 0);
				table.grid.addColumn(30);
				table.grid.add(new TableItem(aName), flop.aIndex, 0);
				table.grid.addColumn(30);
				table.grid.add(new TableItem(bName), flop.bIndex, 0);
				index += 1;
			}
			try{
				index = SRAndJK[SRAndJK.Count - 1].bIndex + 1;
			}catch(Exception e){
				index = inputs.Count;
			}
			foreach (Output flop in outputs) {
				flop.nowIndex = index;
				table.grid.addColumn(30);
				table.grid.add(new TableItem(flop.name), flop.nowIndex, 0);
				index += 1;
			}
		}
		
		public void fill(Table table){
			for (int i = 0; i < (Math.Pow(2.0, inputs.Count + SRAndJK.Count)); i++) {
				var bin = Util.toBin(inputs.Count + SRAndJK.Count, i);
				table.grid.addRow(30);
				for (int j = 0; j < bin.Length; j++) {
					table.grid.add(new TableItem(bin[j].ToString()), j, i + 1);
				}
				for (int k = 0; k < SRAndJK.Count; k++) {
					table.grid.add(new TableItemButton(table, "0"), k + bin.Length, i + 1);
					foreach (FlipFlop f in SRAndJK) {
						try {
							if (f.nextIndex.Equals(k + bin.Length)) {
								var now = table.grid.getChildren(f.nowIndex, i + 1) as TableItem;
								var next = table.grid.getChildren(f.nextIndex, i + 1) as TableItemButton;
								f.setNow(now.Text);
								f.setNext(next.Content.ToString());
								table.grid.add(new TableItem(f.getA()), f.aIndex, i + 1);
								table.grid.add(new TableItem(f.getB()), f.bIndex, i + 1);
								break;
							}
						} catch (Exception e) {
							Debug.Print(e.StackTrace);
						}
					}
				}
				
				int init;
				try{
					init = SRAndJK[SRAndJK.Count - 1].bIndex + 1;
				}catch(Exception e){
					init = inputs.Count;
				}
				for (int l = init; l < init+ outputs.Count; l++) {
					foreach (FlipFlop f in outputs) {
						try {
							if (f.nowIndex.Equals(l)) {
								table.grid.add(new TableItemButton(table, f.getNow()), f.nowIndex, i + 1);
							}
						} catch (Exception e) {
							Debug.WriteLine(e.StackTrace);
						}
					}
				}
			}
		}
	}
}