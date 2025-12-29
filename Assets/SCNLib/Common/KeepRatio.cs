using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.Common
{
	/// <summary>
	/// Giu cho ti le khong bi thay doi
	/// </summary>
	public class KeepRatio : MonoBehaviour
	{
		[SerializeField] RectTransform sampleRect;
		[SerializeField] Mode mode;
		[SerializeField] Vector2 size;

		[SerializeField] bool playAwake = true;

		public RectTransform SampleRect { get => sampleRect; set => sampleRect = value; }
		public Mode Mode1 { get => mode; set => mode = value; }
		public Vector2 Size { get => size; set => size = value; }

		public bool PlayAwake { get => playAwake; set => playAwake = value; }

		RectTransform rectTrans;
		public RectTransform RectTrans
		{
			get
			{
				if (rectTrans == null)
				{
					rectTrans = GetComponent<RectTransform>();
				}

				return rectTrans;
			}
		}

		public enum Mode
		{
			/// <summary>
			/// Object duoc cat tren duoi hoac trai phai de hien thi full theo size cua Sample
			/// </summary>
			FullSample = 0,
			/// <summary>
			/// Object khong bi cat va hien thi full ben trong Sample 
			/// </summary>
			FullObject = 1,
			/// <summary>
			/// Object duoc keo theo chieu rong
			/// </summary>
			FullWidth = 2,
			/// <summary>
			/// Object duoc keo theo chieu cao
			/// </summary>
			FullHeight = 3
		}

		private void Start()
		{
			if (playAwake)
			{
				Resize();
			}
		}

		void Init()
		{

		}

		public void Resize()
		{
			RectTrans.anchorMin = new Vector2(0.5f, 0.5f);
			RectTrans.anchorMax = new Vector2(0.5f, 0.5f);

			switch (mode)
			{
				case Mode.FullSample:
					DoFullSample();
					break;
				case Mode.FullObject:
					DoFullObject();
					break;
				case Mode.FullWidth:
					DoFullWidth();
					break;
				case Mode.FullHeight:
					DoFullHeight();
					break;
				default:
					break;
			}
		}

		void DoFullSample()
		{
			var isHigher = (size.y / size.x) > (sampleRect.rect.height / sampleRect.rect.width);

			if (isHigher)
			{
				DoFullWidth();
			}
			else
			{
				DoFullHeight();
			}
		}

		void DoFullObject()
		{
			var isWider = (size.x / size.y) > (SampleRect.rect.width / SampleRect.rect.height);

			if (isWider)
			{
				DoFullWidth();
			}
			else
			{
				DoFullHeight();
			}
		}

		void DoFullWidth()
		{
			var width = SampleRect.rect.width;

			var scale = width / size.x;
			RectTrans.sizeDelta = new Vector2(width, size.y * scale);
		}

		void DoFullHeight()
		{
			var height = SampleRect.rect.height;

			var scale = height / size.y;
			RectTrans.sizeDelta = new Vector2(size.x * scale, height);
		}
	}
}