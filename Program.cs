using System;
using System.IO;
using System.Text;
namespace txtParse
{
	/****************************************
	//If bid(ask) price or bid(ask) quanty is invalid 
	//I should make it zero so that when I split by space, it will not mess up the position

	*/






	class MainClass
	{	

		private static readonly int updateNum = 0; 
		private static readonly int depth = 1;
		private static readonly int date = 2;
		private static readonly int time = 3;
		private static readonly int bidPrice = 4;
		private static readonly int bidQty = 5;
		private static readonly int askPrice = 6;
		private static readonly int askQty = 7;
		private static readonly int ltp = 8;
		private static readonly int ltq = 9;
		private static readonly int dateNum = 10;
		private static readonly int timeNum = 11;
		private static readonly int volume = 12;

		private StreamWriter outputFile = null;

		private void bidAskHeader(StringBuilder sb, String[] buffer, int type){
			for(int i = 1; i <= 10; i++){
				sb.Append (" ").Append (buffer [type]).Append (i);
			}
		}


		public void reshapeData(String fileName)
		{
			using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (BufferedStream bs = new BufferedStream(fs))
			using (StreamReader sr = new StreamReader(bs))
			{
				string line;
				long lineNum = -1; 
				string[] header = new string[13];
				String outputFileName = string.Format ("ReshapeBook-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
				outputFile = new StreamWriter ("/Users/xiaoxuanshi/Desktop/OrderBook/" + outputFileName);
				outputFile.AutoFlush = true;
				StringBuilder headerContent = new StringBuilder();
				StringBuilder bidP = new StringBuilder ();
				StringBuilder bidQ = new StringBuilder ();
				StringBuilder askP = new StringBuilder ();
				StringBuilder askQ = new StringBuilder ();
				StringBuilder lineResult = new StringBuilder ();
				int level = 0;
				int ax = 0;

				while ((line = sr.ReadLine()) != null)
				{
					
					++lineNum; 

					String[] lineContent = line.Split(null);
					if(lineContent.Length == 12){
						Console.WriteLine ("Warning: there are missing values at line " + lineNum + " for update " + lineContent [updateNum]);
					}

					//first get all the header we need 
					//ONLY DONE ONCE
					if(lineNum == 0){
						for(int i = 0; i < lineContent.Length; i++){
							header[i] = lineContent[i];
						}
						headerContent
							.Append (lineContent [updateNum])
							.Append (" ").Append (lineContent [date])
							.Append (" ").Append (lineContent [time])
							.Append (" ").Append (lineContent [dateNum])
							.Append (" ").Append (lineContent [timeNum]);


						bidAskHeader (headerContent, lineContent, bidPrice);
						bidAskHeader (headerContent, lineContent, bidQty);
						bidAskHeader (headerContent, lineContent, askPrice);
						bidAskHeader (headerContent, lineContent, askQty);

						//volume
						headerContent.Append(" ").Append(lineContent[ltp]).Append(" ").Append(lineContent[ltq]);
						headerContent.Append(" ").Append(lineContent[volume]);

						outputFile.WriteLine (headerContent);
					}

					//updateNumber time
					if(lineNum != 0){
						bidP.Append (" ").Append (lineContent [bidPrice]);
						bidQ.Append (" ").Append (lineContent [bidQty]);
						askP.Append (" ").Append (lineContent [askPrice]);
						askQ.Append (" ").Append (lineContent [askQty]);
						level += 1;


					}

					/*
					 * 
					 * NEED To change the bid hit 
					 *                    ask take 
					 * 
					*/

					//write one update to a file
					if (level == 10) {
						ax += 1;
						//only need to update once
						lineResult
							.Append (lineContent [updateNum])
							.Append (" ").Append (lineContent [date])
							.Append (" ").Append (lineContent [time])
							.Append (" ").Append (lineContent [dateNum])
							.Append (" ").Append (lineContent [timeNum]);
						//assemble the data 
						String temp = lineResult
							.Append (bidP.ToString ())
							.Append (bidQ.ToString ())
							.Append (askP.ToString ())
							.Append (askQ.ToString ())
							.Append (" ").Append (lineContent [ltp])
							.Append (" ").Append (lineContent [ltq])
							.Append (" ").Append (lineContent [volume]).ToString ();
						//write

				
//						Console.WriteLine (temp);


	
						outputFile.WriteLine (temp);
						outputFile.Flush ();

						//clear the buffer
						bidP = new StringBuilder ();
						bidQ = new StringBuilder ();
						askP = new StringBuilder ();
						askQ = new StringBuilder ();
						lineResult = new StringBuilder ();
						level = 0;
					}

				}
				Console.WriteLine (lineNum);
			}

		}


		public static void Main (string[] args)
		{
			MainClass m = new MainClass ();
			m.reshapeData ("/Users/xiaoxuanshi/Dropbox/OrderBook/Book-2016-04-27_01-56-44-AM.txt");

		}
	}
}
