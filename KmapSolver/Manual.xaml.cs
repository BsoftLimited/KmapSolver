/*
 * Created by SharpDevelop.
 * User: okelekele kiensue
 * Date: 4/7/2019
 * Time: 12:29 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;

namespace KmapSolver{
	/// <summary>
	/// Interaction logic for Manual.xaml
	/// </summary>
	/// 
	public delegate void windowsListener(Window Window);
	public partial class Manual : Window{
		public windowsListener WindowsListener;
		
		public Manual(){
			InitializeComponent();
		}
		
		protected override void OnClosed(EventArgs e){
			WindowsListener(this);
			base.OnClosed(e);
		}
	}
}