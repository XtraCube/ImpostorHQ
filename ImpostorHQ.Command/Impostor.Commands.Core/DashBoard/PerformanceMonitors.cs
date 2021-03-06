﻿using System;
using System.Threading;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Impostor.Commands.Core.DashBoard
{
    public class PerformanceMonitors
    {
        /// <summary>
        /// This indicates the CPU usage. We update this value every 1 second. 
        /// </summary>
        public int CpuUsage{ get; private set; }
        /// <summary>
        /// This value does not work and will be permanently set on a random value. (TODO: help needed)
        /// </summary>
        public int MemoryUsage { get; set; }
        public bool Running { get; private set; }
        /// <summary>
        /// The current process.
        /// </summary>
        private Process ProcessCtx { get; set; }
        public PerformanceMonitors()
        {
            this.Running = true;
            this.ProcessCtx = Process.GetCurrentProcess();
            var monThr = new Thread(DoCount);
            monThr.Start();
        }

        private DateTime _startTime, _endTime;
        private TimeSpan _startUsage, _endUsage;
        /// <summary>
        /// This callback updates the status.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DoCount()
        {
            while (Running)
            {
                _startTime = DateTime.Now;
                _startUsage = ProcessCtx.TotalProcessorTime;
                Thread.Sleep(500); //should be good enough for the future.
                _endTime = DateTime.Now;
                _endUsage = ProcessCtx.TotalProcessorTime;
                ProcessCtx.Refresh();
                this.CpuUsage = (int)((((_endUsage - _startUsage).TotalMilliseconds) / (Environment.ProcessorCount * ((_endTime - _startTime).TotalMilliseconds))) * 100);
                this.MemoryUsage = (int) ((ProcessCtx.PrivateMemorySize64 / 1024f) / 1024f);
            }
        }
        public void Shutdown()
        {
            this.Running = false;
        }
    }
}
