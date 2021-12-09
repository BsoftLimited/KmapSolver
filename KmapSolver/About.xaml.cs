/*
 * Created by SharpDevelop.
 * User: okelekele Nobel Bobby
 * Date: 2/10/2019
 * Time: 4:51 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;

namespace KmapSolver{
	public partial class About : Window{
		public windowsListener WindowsListener;
		public About(){
			InitializeComponent();
			text.Text ="KMap Solver version 1.0\n\n"+
				"KMap Solver is a software for generating and evaluating\n" +
				"Truth Table, which can be used to generate and solve KMaps\n" +
				"This software is not intended for profit but for educational \n" +
				"purposes. Any other purposes is punishable by law.\n\n\n"+
				"Bsoft \u00A92019\n\n"+
				"Programmer\n"+
				"Okelekele Nobel Bobby\n\n"+
				"Email: okelekelenobel@gmail.com";
		}
		
		protected override void OnClosed(EventArgs e){
			WindowsListener(this);
			base.OnClosed(e);
		}
	}
}