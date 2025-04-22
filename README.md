[![NuGet](https://img.shields.io/nuget/v/PacketDotNet.svg)](https://www.nuget.org/packages/PacketDotNet/)
[![Build status](https://github.com/chmorgan/packetnet/workflows/master/badge.svg)](https://github.com/chmorgan/packetnet/actions)

Packet.Net
==========

Packet.Net is a high performance .Net assembly for dissecting and constructing
network packets such as Ethernet, IP, TCP, UDP, etc.

Originally created by Chris Morgan <chmorgan@gmail.com>

Performance
======
Packet.Net has been designed for the highest performance possible. As such we aim to perform the most minimal amount of data processing in order to fully determine the datagram nesting.

For example a TCP packet would be parsed into a series of linked objects like: Ethernet -> IPv4 -> TCP but no further data processing is performed until particular fields are accessed. In addition the objects point to packet memory in-place, avoiding allocation and copying of the packet contents unless necessary, such as when altering data payloads or resizing variable length fields.

Test suite
=====
Packet.Net has a comprehensive suite of tests for each of the supported packet types, see the 'Test' subdirectory.

Supported packet formats
=====
* Ethernet
* IPv4 / IPv6
* TCP
* UDP
* ICMP v4 and v6
* IGMP v2 and v3
* L2TP
* PPPoE
* OSPF
* Wake-on-lan
* IEEE 802.1Q
* IEEE 802.1ad
* IEEE 802.11
* DRDA
* ARP
* LLDP
* LSA
* Linux SSL
* PPP
* and probably more, see the source code for the latest list

Capture example
==============
See [Capturing packets example](https://github.com/chmorgan/packetnet/tree/master/Examples/CapturingAndParsingPackets)

<p align="center"><img src="/terminalizer/captureexample.gif?raw=true"/></p>

Getting started
===============

A few basic examples can be found in the Examples/ directory.


Debug vs. Release builds
========================

The Debug build depends on log4net and has log4net calls in some of its classes and
code paths.

The Release build does NOT depend on log4net and, taking advantage of conditional
method attributes, does not include any calls to log4net methods. This ensures that there
is no performance impact on release builds.


Performance benchmarks
======================

The Test/ directory contains a few benchmarks that were used to guide the design
and implementation of Packet.Net. These benchmarks either contain 'performance' or
'benchmark' in their names.

If you have a performance concern or issue you'll want to write a concise test that reproduces
your usage case in a controlled manner. It will then be possible to run and re-run
this test case in various profiling modes in order to look at potential ways of
optimizing code. The tests will also provide a baseline from which to compare
any proposed performance improvements in order to ensure that changes are not
inadvertantly reducing instead of increasing performance.
