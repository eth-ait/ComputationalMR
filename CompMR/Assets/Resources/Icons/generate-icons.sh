#!/bin/bash
#720x720px the icon with colored background 800x800px
#Background from color hashes

COLOR_LIST__2=( "#19aeff" "#ff6600" "#ba00ff" "#9ade00" "#ff4141" "#9eabb0" "#ffff3e" "#b88100" "#57e4e8" "#fff" )
COLOR_LIST=( "#fff" )
WIDTH=128
HEIGHT=128
SVG_DIR=$PWD/svg-icons
OUTPUT=$PWD/png-icons

line='viewBox="0 0 48 48"'
rep='viewBox="-12 -12 72 72"'

CURRENT_DIR=$PWD

mkdir $OUTPUT

cd $SVG_DIR
color=0
name=1;
for i in *; do
  sed -i "s/${line}/${rep}/g" $i
  inkscape -z --file=$i --export-png=$OUTPUT/$name.png --export-area-drawing --export-background=${COLOR_LIST[color]}  --export-width=$WIDTH --export-height=$HEIGHT --export-area-page
  name=$((name+1))

  if [ $((color+1)) -lt ${#COLOR_LIST[@]} ] ; then
    color=$((color+1))
  else
    color=0
  fi
done

cd $CURRENT_DIR

