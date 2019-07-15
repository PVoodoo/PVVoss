#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it.
//
// based to John Ehlers "Cycle Analytics for Traders"
// Sort of direct copy from EasyLanguage Code
// 2019.07.12 v1.0.0.1
// 2019.07.15 v1.0.0.2 Visual enhancements
// Coverted to NinjaTrader marko.rantala@pvoodoo.com https://pvoodoo.com
// License: MIT
//
// AD: if you need any programming for NinjaTrader (Indicator/Strategy/...) Please contact marko.rantala@pvoodoo.com
//


namespace NinjaTrader.NinjaScript.Indicators.PV
{
	public class PVVoss : Indicator
	{
		
		int Order = 0;
		double F1 = 0.0, G1 = 0.0, S1 = 0.0, Bandwidth = 0.25;
		double SumC = 0.0;
		double FiltV = 0.0;
		double VossV = 0.0;
		
		
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Voss Predictive Filter v1.0.0.2. Programming Marko.Rantala@pvoodoo.com";
				Name										= "PVVoss";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				Period					= 20;
				Predict					= 3;
				AddPlot(new Stroke(Brushes.Blue, 2), PlotStyle.Line,  "Voss");
				AddPlot(new Stroke(Brushes.Red, 2), PlotStyle.Line, "Filt");
				
				AddLine(Brushes.Gray,	0,	NinjaTrader.Custom.Resource.NinjaScriptIndicatorZeroLine);
				
			}
			else if (State == State.Configure)
			{
				
				Order = 3*Predict;
				F1 = Math.Cos(2*Math.PI/Period);
				G1 = Math.Cos(Bandwidth*2*Math.PI/Period);
				S1 = 1/G1 - Math.Sqrt(1/(G1*G1) - 1);
			}
		}

		protected override void OnBarUpdate()
		{
			//Add your custom indicator logic here.
			
			if (CurrentBar <= Period || CurrentBar <= 5 || CurrentBar <= Order){
				Voss[0] = 0.0;
				Filt[0] = 0.0;
				return;
			}
			
			FiltV = 0.5 *(1 - S1)*(Close[0] - Close[2]) + F1*(1+S1)*Filt[1] - S1*Filt[2];
			
			// voss predictor
			SumC = 0;
			for (int count = 0; count < Order ; count++)
				SumC = SumC + ((double)(count+1)/Order)*Voss[Order-count];
			
			VossV = ((3 + Order)/2.0)*FiltV - SumC;
			
			Filt[0] = FiltV;
			Voss[0] = VossV;
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Period", Order=1, GroupName="Parameters")]
		public int Period
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Predict", Order=2, GroupName="Parameters")]
		public int Predict
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Voss
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Filt
		{
			get { return Values[1]; }
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private PV.PVVoss[] cachePVVoss;
		public PV.PVVoss PVVoss(int period, int predict)
		{
			return PVVoss(Input, period, predict);
		}

		public PV.PVVoss PVVoss(ISeries<double> input, int period, int predict)
		{
			if (cachePVVoss != null)
				for (int idx = 0; idx < cachePVVoss.Length; idx++)
					if (cachePVVoss[idx] != null && cachePVVoss[idx].Period == period && cachePVVoss[idx].Predict == predict && cachePVVoss[idx].EqualsInput(input))
						return cachePVVoss[idx];
			return CacheIndicator<PV.PVVoss>(new PV.PVVoss(){ Period = period, Predict = predict }, input, ref cachePVVoss);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.PV.PVVoss PVVoss(int period, int predict)
		{
			return indicator.PVVoss(Input, period, predict);
		}

		public Indicators.PV.PVVoss PVVoss(ISeries<double> input , int period, int predict)
		{
			return indicator.PVVoss(input, period, predict);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.PV.PVVoss PVVoss(int period, int predict)
		{
			return indicator.PVVoss(Input, period, predict);
		}

		public Indicators.PV.PVVoss PVVoss(ISeries<double> input , int period, int predict)
		{
			return indicator.PVVoss(input, period, predict);
		}
	}
}

#endregion
