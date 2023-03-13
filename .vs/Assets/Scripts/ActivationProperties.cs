/*  File:       ActivationProperties
    Purpose:    This file contains the Activation properties Inteface.
                Classes that implement this class need to provide a
                set/get for the isActive property. It can be easilly
                accessed generically from any GameObject that implements
                it through GameObject.GetComponent<ActivationProperties>()
    Notes:      More could be added to this that is common among many of the
                classes in this project, and there are some classes written
                by previous developers to which this Interface could be added
                for consistency's sake
    Author:     Ryan Wood
    Created:    Fall 2021
*/

using System;
public interface ActivationProperties
{
    public bool isActive{get; set;}
}

