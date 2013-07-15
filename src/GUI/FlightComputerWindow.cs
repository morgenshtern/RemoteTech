﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RemoteTech {
    public class FlightComputerWindow : AbstractWindow {
        private enum FragmentTab {
            Attitude = 0,
            Rover    = 1,
            Aerial   = 2,
            Progcom  = 3,
        }

        private FragmentTab mTab;
        private const int NUMBER_OF_TABS = 4;
        private readonly AttitudeFragment mAttitude;
        private readonly RoverFragment mRover;
        private readonly AerialFragment mAerial;
#if PROGCOM
        private readonly ProgcomFragment mProgcom;
#endif
        private readonly QueueFragment mQueue;
        private bool mQueueEnabled;
        private readonly VesselSatellite mSatellite;

        private FragmentTab Tab {
            get {
                return mTab;
            }
            set {
                if ((int) value >= NUMBER_OF_TABS) {
                    mTab = (FragmentTab) 0;
                } else if ((int) value < 0) {
                    mTab = (FragmentTab)(NUMBER_OF_TABS - 1);
                } else {
                    mTab = value;
                }
            }
        }

        public FlightComputerWindow(VesselSatellite vs)
            : base("", new Rect(100, 100, 0, 0), WindowAlign.Floating) {
            mSatellite = vs;

            mAttitude = new AttitudeFragment(vs, () => mQueueEnabled = !mQueueEnabled);
            mRover = new RoverFragment();
            mAerial = new AerialFragment();
#if PROGCOM
            mProgcom = new ProgcomFragment(vs);
#endif

            mQueue = new QueueFragment(vs);
            mQueueEnabled = false;
        }

        public override void Window(int id) {
            GUILayout.BeginHorizontal();
            {
                switch (mTab) {
                    case FragmentTab.Attitude:
                        mAttitude.Draw();
                        break;
                    case FragmentTab.Rover:
                        mRover.Draw();
                        break;
                    case FragmentTab.Aerial:
                        mAerial.Draw();
                        break;
#if PROGCOM
                    case FragmentTab.Progcom:
                        mProgcom.Draw();
                        break;
#endif
                }
                if (mQueueEnabled) {
                    mQueue.Draw();
                }
            }
            GUILayout.EndHorizontal();
            if (GUI.Button(new Rect(1, 1, 16, 16), "<")) {
                Tab--;
            }
            if (GUI.Button(new Rect(19, 1, 16, 16), ">")) {
                Tab++;
            }
            base.Window(id);
        }

        protected override void Draw() {
            if (!mSatellite.FlightComputer.InputAllowed) {
                Hide();
            }
            base.Draw();
        }

        public void OnSatelliteUnregister(ISatellite s) {
            Hide();
        }

        public override void Show() {
            RTCore.Instance.Satellites.Unregistered += OnSatelliteUnregister;
            base.Show();
        }

        public override void Hide() {
            base.Hide();
            RTCore.Instance.Satellites.Unregistered -= OnSatelliteUnregister;
        }
    }
}