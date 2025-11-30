reset
#------------------------------------------------------------------------------
set term pngcairo size 600,600 enhanced
set output "mexican_hat_function.png"
#------------------------------------------------------------------------------
set title "Mexican hat function in [-3,3]"

set grid

set pm3d
set palette rgb 33, 13, 10      # rainbow like

set isosamples 25

set xrange [-3:3]
set yrange [-3:3]

set xlabel "x"
set ylabel "y"
set zlabel "z"

sigma  = 0.75
f(x,y) = 1 / (pi * sigma**4) * (1 - (x**2 + y**2) / sigma**2) * exp(-(x**2 + y**2) / (2 * sigma**2))

splot(f(x,y))
