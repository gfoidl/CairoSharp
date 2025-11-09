reset
#------------------------------------------------------------------------------
set datafile separator ";"
#------------------------------------------------------------------------------
set term pngcairo size 1200,600 enhanced
set output "fps_0.png"
#------------------------------------------------------------------------------
unset key
set grid

set title "Naive rendering for animation" font ",16"
set ylabel "FPS"
set xlabel "moves" offset 0,-0.5

plot "fps_0.csv" using 1:2 with lines ls 4
