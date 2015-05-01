﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Thermometer
{
	public class ModuleThermometer : PartModule
	{
		public static readonly String DEGREE_SYMBOL = "\u00B0"; // °
		public static TemperatureUnit tempUnit = TemperatureUnit.CELSIUS;
		public static double percentHighlight = 0.75;

		private Boolean resetTinting = false;

		[KSPField(isPersistant = false, guiActive = true, guiName = "Temperature")]
		public String temperature = "";
		[KSPField(isPersistant = false, guiActive = true, guiName = "Max Temperature")]
		public String maxTemperature = "";

		public override void OnUpdate() {
			temperature = getTemperature(this.part.temperature) + " " + DEGREE_SYMBOL + getUnitString();
			maxTemperature = getTemperature(this.part.maxTemp) + " " + DEGREE_SYMBOL + getUnitString();

			//Thanks to Ferram4 for this highlighting section
			if (this.part.temperature > this.part.maxTemp * percentHighlight) {
				float hP = (float)((this.part.temperature/this.part.maxTemp) * 2.5);
				if (hP > 1) {
					hP = 1;
				}

				Color tintColor = new Color(1,0.1f,0.1f, hP);

				this.part.SetHighlightType (Part.HighlightType.AlwaysOn);
				this.part.SetHighlightColor (tintColor);
				this.part.SetHighlight (true, false);
				resetTinting = true;
			} else if (part.highlightType != Part.HighlightType.OnMouseOver) {
				this.part.SetHighlightType (Part.HighlightType.OnMouseOver);
				this.part.SetHighlightColor (Part.defaultHighlightPart);
				this.part.SetHighlight (false, false);
			} else if (resetTinting) {
				this.part.SetHighlightType (Part.HighlightType.Disabled);
				this.part.SetHighlight (false, true);
				resetTinting = false;
			}
		}

		private double getTemperature(double temp) {
			switch (tempUnit) {
			case TemperatureUnit.CELSIUS:
				return Math.Round(temp, 3) - 273.15;
			case TemperatureUnit.FAHRENHEIT:
				return Math.Round((temp - 273.15)*9/5 + 32, 3);
			case TemperatureUnit.KELVIN:
				return Math.Round(temp, 3);
			}
			return -1;
		}

		private String getUnitString() {
			switch (tempUnit) {
			case TemperatureUnit.CELSIUS:
				return "C";
			case TemperatureUnit.FAHRENHEIT:
				return "F";
			case TemperatureUnit.KELVIN:
				return "K";
			}
			return "ERROR";
		}
	}
}

