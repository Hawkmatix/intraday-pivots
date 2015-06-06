Hawkmatix Intraday Pivots
=========================

Intraday Pivots are among the most powerful type of support and resistance
indicator. They are useful to any style of trader that analyzes a security on
an intraday basis. These pivots are calculated over a period of minute time
values and can be changed to any minute value. The default time value is set to
sixty minutes. For some traders, pivots alone are strong enough to base entries
and exits upon. The standard pivot calculation method has shown the most
correlation with price movement. Other calculation methods are available as
well, which are open included, high weighted, low weighted, and close weighted.
These pivots have five support levels, five resistance levels, and a central
pivot, to provide more zones for extremely volatile periods. The option to
round to the nearest tick is also available.

Installation
------------

Install from source, method 1 (Requires Python):

    1. ``> python setup.py``
    2. Follow the directions after the script completes.

Install from source, method 2:

    1. Unzip the downloaded file ``intraday-pivots-master.zip``.
    2. Locate the source file ``HawkmatixIntradayPivots.cs``.
    3. Move the source file to the NinjaTrader indicator folder ``Documents/
       NinjaTrader 7/bin/Custom/Indicator``.
    4. Open any indicator in NinjaTrader by going to Tools > Edit NinjaScript
       > Indicator...
    5. Press the ``compile`` button in the menu bar.

Package Contents
----------------

    intraday-pivots
        Intraday Pivots sources.

Usage
-----

This software is intended for use with the NinjaTrader trading platform.
Full documentation is available at
http://hawkmatix.github.io/intraday-pivots.html

Supported Operating Environment
-------------------------------

This version of the add-on software has been tested, and is known to work
against the following NinjaTrader versions and operating systems.

NinjaTrader Versions
~~~~~~~~~~~~~~~~~~~~

* NinjaTrader 7.0.1000.27
* NinjaTrader 6.5.1000.19

Operating Systems
~~~~~~~~~~~~~~~~~

* Windows 7/8

Requirements
------------

Supports NinjaTrader 6.5.1000.19 - 7.0.1000.27.

License
-------

All code contained in this repository is Copyright 2012 - Present Andrew C.
Hawkins.

This code is released under the GNU Lesser General Public License. Please see
the COPYING and COPYING.LESSER files for more details.

Contributors
------------

* Andrew C. Hawkins <andrew@hawkmatix.com>

Changelog
---------

* v3 Additional documentation added to the script for clarity.

* v2 The ability to round to the nearest tick is added. The ability to change
  the weighting of the calculation is added.

* v1 Standard intraday pivot calculations are made, based upon an amount of
  time in minutes, and plotted.
