function [Origini] = GenInitPopolazione(PointsCoord, L_Nastro, Step, H_Bordo, L_Cassa, StepHor, D_Polistirolo)
%% Inizializzazione
% Prendo sempre come punto iniziale il punto con ascissa e ordinata minime
% Se nella 3° colonna ho lo zero significa che quel punto è un'origine,
% altrimenti può essere ancora considerato un'origine
Origini(1,1) = 0;
Origini(1,2) = 0;
Origini(1,3) =1;
sxtodx=true; % Ci indica il verso in cui stiamo riempiendo la cassa
i=2;

%% Matrici di traslazione
matx = [1,0,StepHor;0,1,0;0,0,1];
maty = [1,0,0;0,1,StepHor;0,0,1];
matxy = [1,0,StepHor;0,1,StepHor;0,0,1];

%% Controllo
% Devo controllare che in direzione x ci sta ancora un blocchetto + il
% polistirolo
NumeroElementi = L_Nastro / StepHor;
spazioPolistirolo = true;
while size(Origini,1)<NumeroElementi
    if Origini(i-1,1)+D_Polistirolo +StepHor < L_Cassa
        spazioPolistirolo = true;
        Origini(i,:)=matx*Origini(i-1,:)';
    else
        spazioPolistirolo = false;
    end
    i=i+1;
end

%% Numero di elementi in cui viene suddiviso il nastro

i=2;
while size(Origini,1)<NumeroElementi
    
    % Cerchiamo il punto successivo
    if sxtodx == true
        temp(1,1)=Origini(i-1,1)+StepHor;
        temp(1,2) = Origini(i-1,2);
    else
        temp(1,1)=Origini(i-1,1)-StepHor;
        temp(1,2) = Origini(i-1,2);
    end
    
    % Verifica che l'oridinata sia uguale
    if  L_Cassa-temp(1,1) >=20 && L_Cassa-temp(1,1)<=80
        Origini(i,1:2) = temp;
        i=i+1;
    else
        Origini(i,1) = temp(1,1);
        Origini(i,2)=temp(1,2)+H_Bordo/2;
        
        if sxtodx == false
            Origini(i+1,1) = temp(1,1)+StepHor;
            Origini(i+1,2)=temp(1,2)+H_Bordo;
            sxtodx = true;
        else
            Origini(i+1,1) = temp(1,1)-StepHor;
            Origini(i+1,2)=temp(1,2)+H_Bordo;
            sxtodx = false;
        end
        i=i+2;
    end
    
end

end

