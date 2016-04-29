using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridSearch
{
    class Program
    {
        static void Main(String[] args) {
        int t = Convert.ToInt32(Console.ReadLine());
        for(int a0 = 0; a0 < t; a0++){
            string[] tokens_R = Console.ReadLine().Split(' ');
            int R = Convert.ToInt32(tokens_R[0]);
            int C = Convert.ToInt32(tokens_R[1]);
            string[] G = new string[R];
            for(int G_i = 0; G_i < R; G_i++){
               G[G_i] = Console.ReadLine();   
            }
            string[] tokens_r = Console.ReadLine().Split(' ');
            int r = Convert.ToInt32(tokens_r[0]);
            int c = Convert.ToInt32(tokens_r[1]);
            string[] P = new string[r];
            for(int P_i = 0; P_i < r; P_i++){
               P[P_i] = Console.ReadLine();   
            }
            int flag=0;
            for(int i=0;i<R;i++)
            {
                int end = c;
               int start = 0;
                while(start<=C-c)
                {
                    int count=0;
                    if(G[i].Substring(start,end).Equals(P[0]))
                    {
                        for(int j=1; j<r;j++)
                        {
                            if(G[i+j].Substring(start,end).Equals(P[j]))
                                count ++;
                        }
                    }
                    if(count==r-1)
                    {
                        flag=1;
                        break;
                    }
                    start++;
                }
                if(flag==1)
                    break;
            }
            if(flag==1)
                Console.WriteLine("YES");
            else
                Console.WriteLine("NO");
        }
    }
}
