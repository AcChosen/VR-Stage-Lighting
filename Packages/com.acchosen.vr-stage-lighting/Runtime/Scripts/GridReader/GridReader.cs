#if UNITY_EDITOR
using UnityEngine;
using SharpOSC;
using System;

/**

GridReader - An OSC reader for VRSL grid node
TekCastPork

Licensed under MIT
**/

namespace VRSL
{
    public class GridReader : MonoBehaviour
    {
        private UDPListener _Listener;
        [Header("Configuration")]

        [Tooltip("Enable if using the grid node in vertical mode")]
        [SerializeField] bool _IsVertical = true;

        [Tooltip("Enable if you are using RGB mode to use additional universes")]
        [SerializeField] bool _ExpandedUniverseSupport = false;

        [Tooltip("Change if you set a port other than 12000 in the grid node.")]
        [SerializeField] int _OSCPort = 12000;

        [Tooltip("Prefix assigned in the VRSL grid node")]
        [SerializeField] string _OSCPrefix = "/VRSL";

        [Space]

        [Header("INTERNAL STUFF TO MAKE THIS WORK! DON'T EDIT BELOW THIS LINE")]

        [Tooltip("HEY I SAID NO TOUCHY! >:|")]
        [SerializeField] Texture2D _DataBuffer;

        Color[] _Buf;
        bool[] _NeedsUpdate;

        private int _yLim = 67;
        private int _xLim = 13;

        void Start()
        {
            try
            {
                Color _initColor = new Color(0f, 0f, 0f);

                if (!_IsVertical)
                {
                    //Horizontal mode has ~3 universes, adjust limits accordingly
                    _xLim = 120;
                    _yLim = 13;
                }

                //Initialize buffering arrays based on size limits
                _Buf = new Color[_xLim * _yLim];
                _NeedsUpdate = new bool[_xLim * _yLim];

                for (int i = 0; i < _xLim * _yLim; i++)
                {
                    // Ensure arrays are set at some default value
                    _NeedsUpdate[i] = false;
                    _Buf[i] = _initColor;

                }

                // Clear buffer texture
                for(int x = 0; x < _DataBuffer.width; x++)
                {
                    for(int y = 0; y < _DataBuffer.height; y++)
                    {
                        _DataBuffer.SetPixel(x, y, _initColor);
                    }
                }
                _DataBuffer.Apply();


                HandleOscPacket callback = delegate (OscPacket packet)
                {
                    int[] _pktData = new int[4];
                    if (_ExpandedUniverseSupport)
                        TekView.TekParse.ParseExpandedMode(packet, ref _pktData, _OSCPrefix);
                    else
                        TekView.TekParse.ParseStandardMode(packet, ref _pktData, _OSCPrefix);


                    if (_pktData[0] > _Buf.Length) return; // safety return to halt crashing, should cut it to last full channel in DMX-pixel space

                if (_ExpandedUniverseSupport)
                    {
                        _Buf[_pktData[0] - 1].r = _pktData[1] / 255f;
                        _Buf[_pktData[0] - 1].g = _pktData[2] / 255f;
                        _Buf[_pktData[0] - 1].b = _pktData[3] / 255f;
                    }
                    else
                    {
                        _Buf[_pktData[0] - 1].r = _pktData[1] / 255f;
                        _Buf[_pktData[0] - 1].g = _pktData[1] / 255f;
                        _Buf[_pktData[0] - 1].b = _pktData[1] / 255f;
                    }
                    _NeedsUpdate[_pktData[0] - 1] = true;
                };
                try
                {
                    _Listener = new UDPListener(_OSCPort+1, callback);
                }
                catch (Exception e)
                {
                    Debug.LogError("GridReader failed to launch the OSC Listener.");
                    Debug.LogError(e.Message);
                    gameObject.SetActive(false);
                }
                Debug.Log("GridReader Init");
                
                // Request all DMX data from grid node to ensure sync

                UDPSender sender = new UDPSender("127.0.0.1", _OSCPort);
                var message = new OscMessage("/VRSL/Command/RefreshDMX", 0);
                sender.Send(message);
                sender.Close();

            } catch (Exception e)
            {
                Debug.LogError("GridReader failed to launch.");
                Debug.LogError(e.Message);
                gameObject.SetActive(false);
            }

        }

        private void OnApplicationQuit()
        {
            try
            {
                Debug.Log("GridReader Exit");
                _Listener.Close();
            } catch (Exception e)
            {
                Debug.LogError("GridReader failed to close?");
                Debug.LogError(e.Message);
            }
        }

        private void Update()
        {
            try{
            Color[] data = new Color[256];
            int index = 0;
            if (_IsVertical)
            {
                for (int y = 0; y < _yLim; y++)
                {
                    for (int x = 0; x < _xLim; x++)
                    {
                        if (_NeedsUpdate[index])
                        {
                            _NeedsUpdate[index] = false;
                            for (int datInd = 0; datInd < 256; datInd++) data[datInd] = _Buf[index];
                            _DataBuffer.SetPixels(16 * x, (16 * y), 16, 16, data, 0);
                        }
                        index++;
                    }
                }
            } else
            {
                for(int x = 0; x < _xLim; x++)
                {
                    for(int y = 0; y < _yLim; y++)
                    {
                        if (_NeedsUpdate[index])
                        {
                            _NeedsUpdate[index] = false;
                            for (int datInd = 0; datInd < 256; datInd++) data[datInd] = _Buf[index];
                            _DataBuffer.SetPixels(16 * x, (16 * y), 16, 16, data, 0);
                        }
                        index++;
                    }
                }
            }
            _DataBuffer.Apply();
            }
            catch(Exception e)
            {
                Debug.LogError("GridReader failed to run.");
                Debug.LogError(e.Message);
                gameObject.SetActive(false);
            }
        }

    }
}
#endif