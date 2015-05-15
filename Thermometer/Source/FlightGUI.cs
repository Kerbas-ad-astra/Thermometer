using System.IO;
using UnityEngine;

namespace Thermometer
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class FlightGUI : MonoBehaviour
	{
		private Texture2D texture;
		private static ApplicationLauncherButton button;
		private static MainWindow window;
		private static bool isOpen;

		void Start()
		{

			texture = LoadPNG (Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/../app.png");
			if (ApplicationLauncher.Ready) {
				onReady ();
			} else {
				GameEvents.onGUIApplicationLauncherReady.Add (onReady);
			}

			GameEvents.onGUIApplicationLauncherDestroyed.Add (onDestroyed);
			GameEvents.onGUIApplicationLauncherUnreadifying.Add (onUnReady);
		}

		void OnDestroy()
		{
		}

		public void onReady() {
			if (button == null) {
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
				window.initGui (new WindowSettings(100, 100, 200, 250, "ThermometerSettings.cfg"));
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
	}

	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	class Blizzy : MonoBehaviour {
		private IButton button;

		public Blizzy() {
		}

		public void Start() {
			button = ToolbarManager.Instance.add("Thermometer", "ThermometerBlizzyButton");
			button.TexturePath = "Thermometer/blizzy";
			button.ToolTip = "Enable/Disable the Thermometer information panel";
			button.Visibility = new GameScenesVisibility(GameScenes.FLIGHT);
			button.OnClick += e => {FlightGUI.blizzyClick();};
		}

		public void OnDestroy() {
			button.Destroy();
		}
	}
}

