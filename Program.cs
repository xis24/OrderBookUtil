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
		private static readonly int marketSide = 13;

		private StreamWriter outputFile = null;

		private void bidAskHeader (StringBuilder sb, String[] buffer, int type)
		{
			for (int i = 1; i <= 10; i++) {
				sb.Append (" ").Append (buffer [type]).Append (i);
			}
		}


		public void reshapeData (String fileName)
		{
			using (FileStream fs = File.Open (fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			using (BufferedStream bs = new BufferedStream (fs))
			using (StreamReader sr = new StreamReader (bs)) {
				string line;
				long lineNum = -1; 
				string[] header = new string[15];
				String outputFileName = string.Format ("ReshapeBook-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", DateTime.Now);
				outputFile = new StreamWriter ("/Users/xiaoxuanshi/Desktop/OrderBook/" + outputFileName);
				outputFile.AutoFlush = true;
				StringBuilder headerContent = new StringBuilder ();
				StringBuilder bidP = new StringBuilder ();
				StringBuilder bidQ = new StringBuilder ();
				StringBuilder askP = new StringBuilder ();
				StringBuilder askQ = new StringBuilder ();
				StringBuilder auxInfo = new StringBuilder ();
				StringBuilder lastTradeInfo = new StringBuilder ();
				StringBuilder lineResult = new StringBuilder ();
				long updateNumber = 1;
				Boolean halt = false;
				int volMin = int.MaxValue;
				int volMax = int.MinValue;
				int lastTradeQtyMin = int.MaxValue;
				int lastTradeQtyMax = int.MinValue; 
				double lastTradePriceMin = double.MaxValue;
				double lastTradePriceMax = double.MinValue;
				double timeInNumberMin = double.MaxValue;
				double timeInNumberMax = double.MinValue;
				int marketSideNum = 0; 

				while ((line = sr.ReadLine ()) != null){
					line = line.Trim ();
					++lineNum; 

					String[] lineContent = line.Split (null);

					//first get all the header we need 
					//ONLY DONE ONCE
					if (lineNum == 0) {

						if(lineContent.Length != 14){
							foreach(String s in lineContent){
								Console.WriteLine ("** " + s);
							}
							outputFile.Close ();
							throw new Exception ("There are " + lineContent.Length + " headers, and At least one of headers is missing.");
						}

						for (int i = 0; i < lineContent.Length; i++) {
							header [i] = lineContent [i];
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
						headerContent
							.Append (" ").Append (lineContent [ltp])
							.Append (" ").Append (lineContent [ltq])
							.Append (" ").Append (lineContent [volume])
							.Append (" ").Append (lineContent[marketSide])
							.Append (" ").Append ("ttimemi")
							.Append (" ").Append ("ltraprimi")
							.Append (" ").Append ("ltraquami")
							.Append (" ").Append ("volumi")
							.Append (" ").Append ("ttimema")
							.Append (" ").Append ("ltraprima")
							.Append (" ").Append ("ltraquama")
							.Append (" ").Append ("voluma");

						outputFile.WriteLine (headerContent);
						continue;
					}

					long updateNumberArray = Convert.ToInt64 (lineContent [updateNum]);

					//need all update number, date, time, ONLY once
					if (updateNumber == updateNumberArray && !halt) {
						//record the auxilary info 
						auxInfo
							.Append (lineContent [updateNum])
							.Append (" ").Append (lineContent [date])
							.Append (" ").Append (lineContent [time])
							.Append (" ").Append (lineContent [dateNum])
							.Append (" ").Append (lineContent [timeNum]);

						lastTradeInfo
							.Append (" ").Append (lineContent [ltp])
							.Append (" ").Append (lineContent [ltq])
							.Append (" ").Append (lineContent [volume]);

						halt = true;
					}

					//detect the current update is different from last 10 update 
					//start to write last 10 update
					if (updateNumber != updateNumberArray) {
						//since there are three possible 
						if(lineContent[marketSide] == "0"){
							marketSideNum = 0; 
						}else if(lineContent[marketSide] == "Hit"){
							marketSideNum = 1;
						}else{
							marketSideNum = 2;
						}


						lineResult
							.Append (auxInfo.ToString ())
							.Append (bidP.ToString ())
							.Append (bidQ.ToString ())
							.Append (askP.ToString ())
							.Append (askQ.ToString ())
							.Append (lastTradeInfo.ToString ())
							.Append (" ").Append(marketSideNum)

							.Append (" ").Append (timeInNumberMin)
							.Append (" ").Append (lastTradePriceMin)
							.Append (" ").Append (lastTradeQtyMin)
							.Append (" ").Append (volMin)

							.Append (" ").Append (timeInNumberMax)
							.Append (" ").Append (lastTradePriceMax)
							.Append	(" ").Append (lastTradeQtyMax)
							.Append (" ").Append (volMax)

							;
							

						outputFile.WriteLine (lineResult.ToString());
						outputFile.Flush ();

						//clear the buffer
						bidP.Clear();
						bidQ.Clear();
						askP.Clear();
						askQ.Clear();
						auxInfo.Clear ();
						lastTradeInfo.Clear ();
						lineResult.Clear();


						//reset value
						volMin = int.MaxValue;
						volMax = int.MinValue;
						lastTradePriceMin = double.MaxValue;
						lastTradePriceMax = double.MinValue;
						lastTradeQtyMin = int.MaxValue;
						lastTradeQtyMax = int.MinValue;
						timeInNumberMin = double.MaxValue;
						timeInNumberMax = double.MinValue;

						updateNumber = updateNumberArray;
						halt = false;
					}


					//update min and max
					volMin = Math.Min(Convert.ToInt32 (lineContent[volume]), volMin);
					volMax = Math.Max (Convert.ToInt32 (lineContent [volume]), volMax);
					lastTradeQtyMin = Math.Min (Convert.ToInt32(lineContent [ltq]), lastTradeQtyMin);
					lastTradeQtyMax = Math.Max (Convert.ToInt32 (lineContent [ltq]), lastTradeQtyMax);
					lastTradePriceMin = Math.Min (Convert.ToDouble (lineContent [ltp]), lastTradePriceMin);
					lastTradePriceMax = Math.Max (Convert.ToDouble (lineContent [ltp]), lastTradePriceMax);
					timeInNumberMin = Math.Min(Convert.ToDouble(lineContent[timeNum]), timeInNumberMin);
					timeInNumberMax = Math.Max (Convert.ToDouble (lineContent [timeNum]), timeInNumberMax);


					//updateNumber time
					bidP.Append (" ").Append (lineContent [bidPrice] == " " ? "0" : lineContent [bidPrice]);
					bidQ.Append (" ").Append (lineContent [bidQty] == " " ? "0" : lineContent [bidQty]);
					askP.Append (" ").Append (lineContent [askPrice] == " " ? "0" : lineContent [askPrice]);
					askQ.Append (" ").Append (lineContent [askQty] == " " ? "0" : lineContent [askQty]);


					/*
					 * 
					 * NEED To change the bid hit 
					 *                    ask take 
					 * 
					*/

				}
				outputFile.Close ();
				Console.WriteLine (lineNum);
			}

		}


		public static void Main (string[] args)
		{
			MainClass m = new MainClass ();
			m.reshapeData ("/Users/xiaoxuanshi/Dropbox/OrderBook/Book-2016-05-02_04-00-56-PM.txt");

		}
	}
}
