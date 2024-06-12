import React, { Component } from 'react';
import {EmailForm} from "./EmailForm";

export class Home extends Component {
  static displayName = Home.name;

  render() {
    return (
      <div>
            <EmailForm/>
      </div>
    );
  }
}
