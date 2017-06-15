#!/bin/sh

zip -j gcd.zip gcd/bin/Release/gcd.exe

f="readme.txt"
cp README.md $f
echo "[Source](https://github.com/yanxyz/gcd)" >> $f
zip -ml gcd.zip $f
