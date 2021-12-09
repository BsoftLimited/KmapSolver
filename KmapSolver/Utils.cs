using System;

namespace KmapSolver.Utils{
	static class Util {
		public static String DecimalToBin(int number){
			String output="";
			while(number>0){
				output= (number % 2) + output;
				number/=2;
			}
			return output;
		}
		
        public static String toBin(int number, int index){
            var bin=DecimalToBin(index);
            while (bin.Length<number){
				bin = "0"+bin;
            }
            return bin;
        }
    }
}
