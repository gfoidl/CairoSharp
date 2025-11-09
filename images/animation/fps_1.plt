reset
#------------------------------------------------------------------------------
set datafile separator ";"
#------------------------------------------------------------------------------
set term pngcairo size 1200,600 enhanced
set output "fps_1.png"
#------------------------------------------------------------------------------
unset key
set grid

set title "Optimized rendering for animation" font ",16"
set ylabel "FPS"
set xlabel "moves" offset 0,-0.5

plot "fps_1.csv" using 1:2 with lines ls 2
