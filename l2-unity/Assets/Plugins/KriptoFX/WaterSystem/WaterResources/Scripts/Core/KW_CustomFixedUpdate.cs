using UnityEngine;
using System.Collections;

namespace KWS
{
	public class KW_CustomFixedUpdate
	{
		public delegate void OnFixedUpdateCallback();
		private float m_Timer = 0;
		int MaxAllowedFrames;

		private OnFixedUpdateCallback m_Callback;

		public KW_CustomFixedUpdate(OnFixedUpdateCallback aCallback, int maxAllowedFrames)
		{
			m_Callback = aCallback;
			MaxAllowedFrames = maxAllowedFrames;
		}

		public void Update(float currentDeltaTime, float floatFixedDeltaTime)
		{
			m_Timer += currentDeltaTime;
			int frames = 0;
			while (m_Timer > 0f)
			{
				m_Callback();
				m_Timer -= floatFixedDeltaTime;
				frames++;
				if (frames > MaxAllowedFrames)
				{
					m_Timer = 0;
					break;
				}
			}
		}

	}
}