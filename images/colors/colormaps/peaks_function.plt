reset
#------------------------------------------------------------------------------
set term pngcairo size 600,600 enhanced
set output "peaks_function.png"
#------------------------------------------------------------------------------
set title "Peaks function in [-3,3]"

set grid

set pm3d
set palette rgb 33, 13, 10      # rainbow like

set isosamples 25

set xrange [-3:3]
set yrange [-3:3]

set xlabel "x"
set ylabel "y"
set zlabel "z"

f(x,y) = 3 * (1-x)**2 * exp(-x**2 - (y+1)**2) - 10 * (x/5 - x**3 - y**5) * exp(-x**2 - y**2) - 1/3 * exp(-(x+1)**2 - y**2)

splot(f(x,y))
