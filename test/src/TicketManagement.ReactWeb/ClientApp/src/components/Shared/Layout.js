import React, { Component } from 'react';
import { Header } from '../Modules/Header';
import { Footer } from '../Modules/Footer';

export class Layout extends Component {
    static displayName = Layout.name;
  render () {
    return (
        <>
            <Header />
            <main>
                {this.props.children}
            </main>
            <Footer />
        </>
  );
  }
}
