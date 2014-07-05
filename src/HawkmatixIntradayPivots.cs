/*
 * Hawkmatix Intraday Pivots 1.0.0.3
 * Official project page: http://www.hawkmatix.com/projects/idpivots
 *
 * Copyright 2012, 2013 Andrew Hawkins
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

// Using declaration
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Gui.Chart;

// User menu choices
public enum RoundToTick
{
	Yes,
	No
}
public enum CentralPivotCalculation
{
	Standard,
	OpenIncluded,
	CloseWeighted,
	HighWeighted,
	LowWeighted,
}

// Begin indicator script
namespace NinjaTrader.Indicator
{
	// Description
	[Description("Hawkmatix Intraday Pivots are points of support and resistance calculated at a time frame less than the entire day.")]

	// Begin class script
	public class HawkmatixIntradayPivots : Indicator
	{
		// Declare class variables
		private Bars periodBars;
		private int timeFrame	= 60;
		private bool existsHist	= false;
		private bool full	= false;
		private bool apt	= false;
		private DateTime sessionBegin;
		private DateTime sessionEnd;
		private RoundToTick roundToTick 			= RoundToTick.Yes;
		private CentralPivotCalculation centralPivotCalculation = CentralPivotCalculation.Standard;

		// Override Initialize method
		protected override void Initialize()
		{
			// Add period of calculation
			Add(PeriodType.Minute, timeFrame);

			// Add plots
			Add(new Plot(new Pen(Color.DarkRed, 3), PlotStyle.Hash, "Resistance 5"));
			Add(new Plot(new Pen(Color.DarkRed, 3), PlotStyle.Hash, "Resistance 4"));
			Add(new Plot(new Pen(Color.DarkRed, 3), PlotStyle.Hash, "Resistance 3"));
			Add(new Plot(new Pen(Color.DarkRed, 3), PlotStyle.Hash, "Resistance 2"));
			Add(new Plot(new Pen(Color.DarkRed, 3), PlotStyle.Hash, "Resistance 1"));
			Add(new Plot(new Pen(Color.SlateGray, 3), PlotStyle.Hash, "Central Pivot"));
			Add(new Plot(new Pen(Color.DarkGreen, 3), PlotStyle.Hash, "Support 1"));
			Add(new Plot(new Pen(Color.DarkGreen, 3), PlotStyle.Hash, "Support 2"));
			Add(new Plot(new Pen(Color.DarkGreen, 3), PlotStyle.Hash, "Support 3"));
			Add(new Plot(new Pen(Color.DarkGreen, 3), PlotStyle.Hash, "Support 4"));
			Add(new Plot(new Pen(Color.DarkGreen, 3), PlotStyle.Hash, "Support 5"));

			// Default parameters
			Overlay				= true;
			CalculateOnBarClose = false;
			AutoScale           = false;
			MaximumBarsLookBack = MaximumBarsLookBack.Infinite;
		}

		// Override OnBarUpdate method
		protected override void OnBarUpdate()
		{
			// Protect against too few bars
			if (Bars == null)
			{
				return;
			}
			if (!Data.BarsType.GetInstance(Bars.Period.Id).IsIntraday)
			{
				return;
			}
			if (Bars.Period.Id == PeriodType.Minute && Bars.Period.Value > timeFrame / 2)
			{
				return;
			}
			if(!full && !apt)
			{
				apt = true;
				periodBars = Data.Bars.GetBars(Bars.Instrument, new Period(PeriodType.Minute, timeFrame, MarketDataType.Last), Bars.From, Bars.To, (Session)Bars.Session.Clone(), Data.Bars.SplitAdjust, Data.Bars.DividendAdjust);
				apt = false;
				full = true;
			}

			// Remove calculation for first time span after open
			IBar periodBar;
			Bars.Session.GetNextBeginEnd(Time[0], out sessionBegin, out sessionEnd);
			if (Time[0] >= sessionBegin && Time[0] <= sessionBegin.AddMinutes(timeFrame))
			{
				Pivot.Reset();
				Resistance1.Reset();
				Support1.Reset();
				Resistance2.Reset();
				Support2.Reset();
				Resistance3.Reset();
				Support3.Reset();
				Resistance4.Reset();
				Support4.Reset();
				Resistance5.Reset();
				Support5.Reset();
			}
			else
			{
				// Determine open, high, low, and close
				DateTime intradayBarTime = Time[0].AddMinutes(-timeFrame);
				periodBar = periodBars.Get(periodBars.GetBar(intradayBarTime));
				double Open  = periodBar.Open;
				double High  = periodBar.High;
				double Low 	 = periodBar.Low;
				double Close = periodBar.Close;
				double pivot = periodBar.Close;

				// Switch central pivot calculation
				switch (centralPivotCalculation)
				{
					// Standard calculation case
					case CentralPivotCalculation.Standard:
					{
						pivot = (High + Low + Close) / 3;
						break;
					}
					// Open included calculation case
					case CentralPivotCalculation.OpenIncluded:
					{
						pivot = (Open + High + Low + Close) / 4;
						break;
					}
					// Close weighted calculation case
					case CentralPivotCalculation.CloseWeighted:
					{
						pivot = (High + Low + Close + Close) / 4;
						break;
					}
					// High weighted calculation case
					case CentralPivotCalculation.HighWeighted:
					{
						pivot = (High + High + Low + Close) / 4;
						break;
					}
					// Low weighted calculation case
					case CentralPivotCalculation.LowWeighted:
					{
						pivot = (High + Low + Low + Close) / 4;
						break;
					}
				}

				// Switch round to tick option
				switch (roundToTick)
				{
					// Rounding case
					case RoundToTick.Yes:
					{
						// Set all additional pivots, rounded
						Pivot.Set(Instrument.MasterInstrument.Round2TickSize(pivot));
						Resistance1.Set(Instrument.MasterInstrument.Round2TickSize(2 * Pivot[0] - Low));
						Support1.Set(Instrument.MasterInstrument.Round2TickSize(2 * Pivot[0] - High));
						Resistance2.Set(Instrument.MasterInstrument.Round2TickSize(Pivot[0] + (High - Low)));
						Support2.Set(Instrument.MasterInstrument.Round2TickSize(Pivot[0] - (High - Low)));
						Resistance3.Set(Instrument.MasterInstrument.Round2TickSize(High + 2 * (Pivot[0] - Low)));
						Support3.Set(Instrument.MasterInstrument.Round2TickSize(Low - 2 * (High - Pivot[0])));
						Resistance4.Set(Instrument.MasterInstrument.Round2TickSize(Pivot[0] + 2 * (High - Low)));
						Support4.Set(Instrument.MasterInstrument.Round2TickSize(Pivot[0] - 2 * (High - Low)));
						Resistance5.Set(Instrument.MasterInstrument.Round2TickSize(2 * (Pivot[0] + High) - 3 * Low));
						Support5.Set(Instrument.MasterInstrument.Round2TickSize(2 * (Pivot[0] + Low) - 3 * High));
						break;
					}
					// No rounding case
					case RoundToTick.No:
					{
						// Set all additional pivots, not rounded
						Pivot.Set(pivot);
						Resistance1.Set(2 * Pivot[0] - Low);
						Support1.Set(2 * Pivot[0] - High);
						Resistance2.Set(Pivot[0] + (High - Low));
						Support2.Set(Pivot[0] - (High - Low));
						Resistance3.Set(High + 2 * (Pivot[0] - Low));
						Support3.Set(Low - 2 * (High - Pivot[0]));
						Resistance4.Set(Pivot[0] + 2 * (High - Low));
						Support4.Set(Pivot[0] - 2 * (High - Low));
						Resistance5.Set(2 * (Pivot[0] + High) - 3 * Low);
						Support5.Set(2 * (Pivot[0] + Low) - 3 * High);
						break;
					}
				}		
			}
		}

		// Properties of plots and inputs
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Resistance5
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Resistance4
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Resistance3
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Resistance2
		{
			get { return Values[3]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Resistance1
		{
			get { return Values[4]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Pivot
		{
			get { return Values[5]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Support1
		{
			get { return Values[6]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Support2
		{
			get { return Values[7]; }
		}
		
		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Support3
		{
			get { return Values[8]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Support4
		{
			get { return Values[9]; }
		}

		[Browsable(false)]
		[XmlIgnore()]
		public DataSeries Support5
		{
			get { return Values[10]; }
		}
		
		[Description("Time frame, in minutes, used to calculate pivots.")]
		[GridCategory("Parameters")]
		[Gui.Design.DisplayNameAttribute("Time Frame")]
		public int TimeFrame
		{
			get { return timeFrame; }
			set { timeFrame = Math.Max(1, value); }
		}

		[Description("Rounding to the nearest tick.")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Round to Tick")]
		public RoundToTick RoundToTick
		{
			get { return roundToTick; }
			set { roundToTick = value; }
		}

		[Description("Type of calculation for the central pivot.")]
		[Category("Parameters")]
		[Gui.Design.DisplayNameAttribute("Central Pivot Calculation")]
		public CentralPivotCalculation CentralPivotCalculation
		{
			get { return centralPivotCalculation; }
			set { centralPivotCalculation = value; }
		}
	}
}
