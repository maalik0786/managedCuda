﻿//	Copyright (c) 2012, Michael Kunz. All rights reserved.
//	http://kunzmi.github.io/managedCuda
//
//	This file is part of ManagedCuda.
//
//	ManagedCuda is free software: you can redistribute it and/or modify
//	it under the terms of the GNU Lesser General Public License as 
//	published by the Free Software Foundation, either version 2.1 of the 
//	License, or (at your option) any later version.
//
//	ManagedCuda is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//	GNU Lesser General Public License for more details.
//
//	You should have received a copy of the GNU Lesser General Public
//	License along with this library; if not, write to the Free Software
//	Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston,
//	MA 02110-1301  USA, http://www.gnu.org/licenses/.


using System;
using System.Text;
using System.Diagnostics;
using ManagedCuda.BasicTypes;

namespace ManagedCuda.CudaSparse
{
	/// <summary>
	/// 
	/// </summary>
	public class DenseMatrix<dataT> : IDisposable where dataT : struct
	{
		private cusparseStatus res;
		private bool disposed;
		private cusparseDnMatDescr descr;
		private cudaDataType typeData;
		private long rows;
		private long cols;
		private long ld;
		private Order order;



		#region Contructors
		/// <summary>
		/// </summary>
		public DenseMatrix(long aRows, long aCols, long aLd, Order aOrder, CudaDeviceVariable<dataT> values)
		{
			rows = aRows;
			cols = aCols;
			ld = aLd;
			order = aOrder;
			descr = new cusparseDnMatDescr();
			typeData = CudaDataTypeTranslator.GetType(typeof(dataT));
			res = CudaSparseNativeMethods.cusparseCreateDnMat(ref descr, rows, cols, ld, values.DevicePointer, typeData, order);
			Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "cusparseCreateDnMat", res));
			if (res != cusparseStatus.Success)
				throw new CudaSparseException(res);
		}

		/// <summary>
		/// For dispose
		/// </summary>
		~DenseMatrix()
		{
			Dispose(false);
		}
		#endregion

		#region Dispose
		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// For IDisposable
		/// </summary>
		/// <param name="fDisposing"></param>
		protected virtual void Dispose(bool fDisposing)
		{
			if (fDisposing && !disposed)
			{
				//Ignore if failing
				res = CudaSparseNativeMethods.cusparseDestroyDnMat(descr);
				Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "cusparseDestroyDnMat", res));
				disposed = true;
			}
			if (!fDisposing && !disposed)
				Debug.WriteLine(String.Format("ManagedCUDA not-disposed warning: {0}", this.GetType()));
		}
		#endregion

		/// <summary>
		/// Returns the inner handle.
		/// </summary>
		public cusparseDnMatDescr Descr
		{
			get { return descr; }
		}
		/// <summary>
		/// 
		/// </summary>
		public cudaDataType TypeData
		{
			get { return typeData; }
		}
		/// <summary>
		/// 
		/// </summary>
		public long Rows
		{
			get { return rows; }
		}
		/// <summary>
		/// 
		/// </summary>
		public long Cols
		{
			get { return cols; }
		}
		/// <summary>
		/// 
		/// </summary>
		public long Ld
		{
			get { return ld; }
		}
		/// <summary>
		/// 
		/// </summary>
		public Order Order
		{
			get { return order; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public CudaDeviceVariable<dataT> GetValues()
		{
			CUdeviceptr devPtr = new CUdeviceptr();
			res = CudaSparseNativeMethods.cusparseDnMatGetValues(descr, ref devPtr);
			Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "cusparseDnMatGetValues", res));
			if (res != cusparseStatus.Success)
				throw new CudaSparseException(res);
			return new CudaDeviceVariable<dataT>(devPtr);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="data"></param>
		public void SetValues(CudaDeviceVariable<dataT> data)
		{
			res = CudaSparseNativeMethods.cusparseDnMatSetValues(descr, data.DevicePointer);
			Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "cusparseDnMatSetValues", res));
			if (res != cusparseStatus.Success)
				throw new CudaSparseException(res);
		}

		/// <summary>
		/// 
		/// </summary>
		public CudaDeviceVariable<dataT> Get()
		{
			CUdeviceptr ptrValues = new CUdeviceptr();

			res = CudaSparseNativeMethods.cusparseDnMatGet(descr, ref rows, ref cols, ref ld, ref ptrValues, ref typeData, ref order);
			Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "cusparseDnMatGet", res));
			if (res != cusparseStatus.Success)
				throw new CudaSparseException(res);

			return new CudaDeviceVariable<dataT>(ptrValues);
		}

		/// <summary>
		/// 
		/// </summary>
		public void GetStridedBatch(ref int batchCount, ref long batchStride)
		{
			res = CudaSparseNativeMethods.cusparseDnMatGetStridedBatch(descr, ref batchCount, ref batchStride);
			Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "cusparseDnMatGetStridedBatch", res));
			if (res != cusparseStatus.Success)
				throw new CudaSparseException(res);
		}

		/// <summary>
		/// 
		/// </summary>
		public void SetStridedBatch(int batchCount, long batchStride)
		{
			res = CudaSparseNativeMethods.cusparseDnMatSetStridedBatch(descr, batchCount, batchStride);
			Debug.WriteLine(String.Format("{0:G}, {1}: {2}", DateTime.Now, "cusparseDnMatSetStridedBatch", res));
			if (res != cusparseStatus.Success)
				throw new CudaSparseException(res);
		}
	}
}
