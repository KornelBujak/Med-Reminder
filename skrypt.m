clc;clear all;close all;

[y,Fs]=audioread("przyklad_do_usuniecia_ciszy.wav");

sound(y,Fs);

tr = 25;                             
tn = 10;                            
p = 60;                            

y=(y/max(y));
nwind = round(Fs * tr / 1000);
nx = length(y);
nframes_no = floor(nx / nwind);
x_trimmed = y(1:nframes_no * nwind);

Y_no = reshape(x_trimmed, nwind, nframes_no);
energy_no = sum(Y_no.^2) / nwind;
thresh_no = 10^(-p / 20);
active_no = energy_no > thresh_no;
Y_no_active = Y_no(:, active_no);
x_no_silence = Y_no_active(:);

noverlap = round(Fs * tn / 1000);
step = nwind - noverlap;
nframes_ov = floor((nx - noverlap) / step);
xp = (0:(nframes_ov - 1)) * step;
xs = (1:nwind)';
colindices = repmat(xp, nwind, 1);
rowindices = repmat(xs, 1, nframes_ov);
indices = rowindices + colindices;

Y_ov = y(indices);
energy_ov = sum(Y_ov.^2) / nwind;
thresh_ov = 10^(-p / 20);
active_ov = energy_ov > thresh_ov;
indices_active = indices(:, active_ov);
zostaja = unique(indices_active(:));
x_ov_silence = y(zostaja);

t = (0:length(y)-1)/Fs;
t_no = (0:length(x_no_silence)-1)/Fs;
t_ov = (0:length(x_ov_silence)-1)/Fs;

figure;
subplot(3,1,1);
plot(t, y);
title('Sygnał oryginalny'); 
xlabel('Czas [s]'); 
ylabel('Amplituda');

subplot(3,1,2);
plot(t_no, x_no_silence);
title('Bez ciszy (bez nakładania)'); 
xlabel('Czas [s]'); 
ylabel('Amplituda');

subplot(3,1,3);
plot(t_ov, x_ov_silence);
title('Bez ciszy (z nakładaniem)'); 
xlabel('Czas [s]'); 
ylabel('Amplituda');

sound(x_ov_silence, Fs);
audiowrite("bez_ciszy_bez_nakladania.wav", x_no_silence, Fs);
audiowrite("bez_ciszy_z_nakladaniem.wav", x_ov_silence, Fs);
