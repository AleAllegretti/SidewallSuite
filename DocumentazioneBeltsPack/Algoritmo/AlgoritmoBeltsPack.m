clc, clear all, close all;
%% Parametri iniziali cassa + nastro
L_Cassa = 100;
H_Cassa = 100;
L_Nastro = 1500;
H_Bordo = 20;
D_Corrugato = 400;
D_Polistirolo = 20;
X_Coord(1,1)=0;
Y_Coord(1,2)=0;
Step=50;
L_NastroImballato = 0;
n=2;
PointsCoord = zeros(122,2);
StepHor = 10;

%% Parametri iniziali algoritmo
itermax = 50;

%% Risoluzione problema
% while L_NastroImballato < L_Nastro
%     % Posiziona i blocchetti in modo consecutivo
%     if X_Coord(n,1)+D_Polistirolo < L_Cassa
%         X_Coord(n,1) = X_Coord(n-1,1)+Step;
%         Y_Coord(n,2)= Y_Coord(n-1,2);
%     end
% end

%% Discretizzazione del dominio
Coord(1:11,1) = linspace(0,L_Cassa,11);
Coord(1:11,2) = linspace(0,H_Cassa,11);
figure(1)
rectangle('position',[0 0 L_Cassa H_Cassa]);
hold on
k=1;
% Genero le coordinate dei nodi (possibili origini)
for j=1:size(Coord,1)
    for i=1:size(Coord,1)
        PointsCoord(k,1) = Coord(i,1);
        PointsCoord(k,2) = Coord(j,2);
        PointsCoord(k,3)=1;
        plot(Coord(i,1),Coord(j,2),'ko');  
        k=k+1;
    end
    
end

%% Creazione insieme iniziale di soluzioni
[Origini] = GenInitPopolazione(PointsCoord, L_Nastro, Step, H_Bordo, L_Cassa, StepHor, D_Polistirolo);

%% Plot dei cerchi
figure(1)
plot(Origini(:,1),Origini(:,2),'ro','MarkerSize',120);