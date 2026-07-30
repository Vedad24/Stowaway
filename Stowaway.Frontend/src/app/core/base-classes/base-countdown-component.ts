import { Directive, inject, OnDestroy, OnInit, signal } from "@angular/core";
import { Router } from "@angular/router";
import { interval, Subscription, takeWhile } from "rxjs";

@Directive()
export abstract class BaseCountdownRedirectComponent implements OnInit, OnDestroy{
    public countdownDuration = signal(5); //default is 5 seconds
    protected abstract targetUrl: any[]; //abstract means it will have to be "overriden" in extended components 
    private timerSubscription! : Subscription;
    protected router = inject(Router);
    
    // constructor(countdownDuration: number = 5, targetUrl: any[] = ["/main"]) {
    //    this.countdownDuration = countdownDuration;
    //    this.targetUrl = targetUrl;
    // }
    ngOnInit() {
        this.startCountdown();
    }

    private startCountdown() {
        this.timerSubscription = interval(1000)
        .pipe(takeWhile(() => this.countdownDuration() > 0))
        .subscribe({
            next: () => {
            this.countdownDuration.update(c => c = c - 1);
            if (this.countdownDuration() === 0) {
                this.router.navigate(this.targetUrl);
            }
            }
        });
    }

    ngOnDestroy() {
        if (this.timerSubscription) {
        this.timerSubscription.unsubscribe();
        }
    }


}
