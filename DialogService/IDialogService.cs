﻿using System;
using System.Threading.Tasks;

namespace AutoRingUAB
{
    public interface IDialogService
    {
        void ShowProgressDialog(string message, Func<ISimpleProgress, Task> workAsync);
        void ShowProgressDialog(string message, int maximum, Func<ISimpleProgress, Task> workAsync);
    }
}