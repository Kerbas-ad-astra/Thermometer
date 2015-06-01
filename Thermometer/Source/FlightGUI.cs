using System.IO;
using UnityEngine;

namespace Thermometer
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class FlightGUI : MonoBehaviour
	{
		private Texture2D texture;
		private static ApplicationLauncherButton button;
		public static MainWindow window;
		private static bool isOpen;
		public static FlightGUI instance;
		private static WindowSettings windowSettings;

		void Start()
		{
			windowSettings = new WindowSettings (100, 100, 200, 250, "ThermometerSettings.cfg");
			windowSettings.Load ();

			FlightGUI.instance = this;

			Debug.Log ("INSTANCE == this? " + (FlightGUI.instance == this));

			texture = LoadPNG (Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/../app.png");
			if (ApplicationLauncher.Ready) {
				onReady ();
			} else {
				GameEvents.onGUIApplicationLauncherReady.Add (onReady);
			}

			GameEvents.onGUIApplicationLauncherDestroyed.Add (onDestroyed);
			GameEvents.onGUIApplicationLauncherUnreadifying.Add (onUnReady);

			updateVisibilityStatus (windowSettings);
		}

		void OnDestroy()
		{
		}

		public void onReady() {
			if (button == null && windowSettings.isStockAppEnabled) {
				button = ApplicationLauncher.Instance.AddModApplication (onTrue, onFalse, onHover, onHoverOut, onEnable, onDisable, ApplicationLauncher.AppScenes.FLIGHT, texture);
			}
		}

		public void onDestroyed() {
			button = null;
			isOpen = false;
		}
		public void onUnReady(GameScenes scene) {
			if (button != null) {
				ApplicationLauncher.Instance.RemoveModApplication (button);
			}
			if (window != null) {
				onFalse ();
			}
			onDestroyed ();
		}

		public static void onTrue() {
			if (window == null) {
				isOpen = true;
				window = new MainWindow ();
				window.initGui (windowSettings);
			}
		}
		public static void onFalse() {
			window.remove ();
			window = null;
			isOpen = false;
		}
		public void onHover() {
		}
		public void onHoverOut() {
		}
		public void onEnable() {
		}
		public void onDisable() {
		}

		public static Texture2D LoadPNG(string filePath) {
			Texture2D tex = null;
			byte[] fileData;

			if (File.Exists(filePath))     {
				fileData = File.ReadAllBytes(filePath);
				tex = new Texture2D(2, 2);
				tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
			}
			return tex;
		}

		public static void blizzyClick() {
			if (FlightGUI.button != null) {
				if(FlightGUI.isOpen){
					FlightGUI.button.SetFalse();
				} else {
					FlightGUI.button.SetTrue();
				}
			} else {
				if (isOpen) {
					onFalse ();
				} else {
					onTrue ();
				}
			}
		}

		public static void updateVisibilityStatus(WindowSettings settings) {
			if (settings.isStockAppEnabled) {
				FlightGUI.instance.onReady ();
			} else {
				if (button != null) {
					ApplicationLauncher.Instance.RemoveModApplication (button);
				}
			}
		}
	}

	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	class Blizzy : MonoBehaviour {
		private static IButton button;
		public static Blizzy instance;

		public Blizzy() {
			Blizzy.instance = this;
		}

		public void Start() {
			if (ToolbarManager.ToolbarAvailable) {
				button = ToolbarManager.Instance.add ("Thermometer", "ThermometerBlizzyButton");
				button.TexturePath = "Thermometer/blizzy";
				button.ToolTip = "Enable/Disable the Thermometer information panel";
				button.Visibility = new GameScenesVisibility (GameScenes.FLIGHT);
				button.OnClick += e => {
					FlightGUI.blizzyClick ();
				};
			}
		}

		public void OnDestroy() {
			if (button != null) {
				button.Destroy ();
			}
		}
	}
}

